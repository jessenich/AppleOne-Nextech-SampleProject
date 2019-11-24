using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

                var topStoryIds = _hackerNewsClient.GetTopStoriesAsync().Result;
                await Task.Delay(1000);
                var newStoryIds = _hackerNewsClient.GetNewStoriesAsync().Result;

                var orderedIds = topStoryIds
                    //.Concat(bestStoryIds)
                    .Concat(newStoryIds)
                    .Distinct()
                    .AsQueryable()
                    .OrderBy(id => id);

                var pager = new AsynchronousTaskPager<int, HackerNewsItemsResponseModel>(orderedIds, logger);
                pager.ResultSelector = id => _hackerNewsClient.GetStoryByIdAsync(id, cancellationToken);
                await pager.RunAsync();
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex.ToString());
            }
        }
    }
}
