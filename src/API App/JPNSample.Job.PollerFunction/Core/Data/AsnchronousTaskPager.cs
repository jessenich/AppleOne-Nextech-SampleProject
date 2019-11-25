using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace JPNSample.API.Core.Data
{
    public class AsynchronousTaskPager<TKey, TResult>
    {
        private readonly IOrderedQueryable<TKey> _query;
        private readonly ILogger _logger;

        public Func<TKey, Task<TResult>> ResultSelector { get; set; }

        public AsynchronousTaskPager(IOrderedQueryable<TKey> query, ILogger logger)
        {
            _query = query ?? throw new ArgumentNullException(nameof(query));
            _logger = logger;
        }

        public async Task RunAsync(int take = 50, int? limit = null)
        {
            var entityCount = _query.Count();
            if (limit.HasValue && limit.Value < entityCount)
            {
                entityCount = limit.Value;
                if (limit.Value < take)
                    take = limit.Value;
            }

            var processed = 0;
            while (processed <= entityCount)
            {
                var entities = _query.Skip(processed)
                                     .Take(take)
                                     .ToList();

                if (entities.Count == 0)
                {
                    _logger?.LogDebug($"Pager reached end with {processed} entities processed");
                    break;
                }

                _logger?.LogDebug($"Pager found {entities.Count} entities at index {processed}");

                await Task.WhenAll(entities.Select(entity => {
                    try
                    {
                        var result =  ResultSelector.Invoke(entity);
                        return result;
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError($"Error occurred executing entity callback at index {processed}\r\n\t{ex.ToString()}");
                        return Task.CompletedTask;
                    }
                }).ToList());

                processed += entities.Count;
                _logger?.LogDebug($"{processed} of {entityCount} entities processed");
            }
        }
    }
}
