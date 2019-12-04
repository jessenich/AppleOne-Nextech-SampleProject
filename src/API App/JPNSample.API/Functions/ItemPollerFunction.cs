using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using JPNSample.API.Core;
using JPNSample.API.Core.Data;
using JPNSample.API.Core.Integration.HackerNews;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace JPNSample.API.Functions
{
    public class ItemPollerFunction
    {
        private readonly IHackerNewsRestClient _hackerNewsClient;

        public ItemPollerFunction(IHackerNewsRestClient hackerNewsClient)
        {
            _hackerNewsClient = hackerNewsClient ?? throw new ArgumentNullException(nameof(hackerNewsClient));
        }

        
        [FunctionName("ItemPoller")]
        public async Task Run(
            [TimerTrigger("%PollerTimerExpression%", RunOnStartup = true)]TimerInfo myTimer, 
            ILogger logger, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                IOrderedQueryable<int> orderedIds = null;
                await Task.WhenAll(
                    _hackerNewsClient.GetTopStoriesAsync(cancellationToken),
                    _hackerNewsClient.GetNewStoriesAsync(cancellationToken),
                    _hackerNewsClient.GetBestStoriesAsync(cancellationToken))
                .ContinueWith(task => 
                    orderedIds = task.Result
                        .SelectMany(model => model.Ids)
                        .Distinct()
                        .AsQueryable()
                        .OrderBy(id => id));

                var pager = new AsynchronousTaskPager<int, HackerNewsStoriesResponseModel>(orderedIds, logger);
                pager.ResultSelector = id => _hackerNewsClient.GetStoryByIdAsync(id, cancellationToken);
                await pager.RunAsync(take: 100, limit: null);
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex.ToString());
            }
        }
    }
}
