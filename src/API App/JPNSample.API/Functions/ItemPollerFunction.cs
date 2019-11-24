using System;
using System.Linq;
using System.Threading;

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
        public void Run(
            [TimerTrigger("%PollerTimerExpression%", RunOnStartup = true)]TimerInfo myTimer, 
            ILogger logger, 
            ExecutionContext executionContext,
            CancellationToken cancellationToken = default)
        {
            //var config = new ConfigurationBuilder()
            //    .SetBasePath(executionContext.FunctionAppDirectory)
            //    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            //    .AddEnvironmentVariables()
            //    .Build();
            try
            {

                var topStoryIds = _hackerNewsClient.GetTopStoriesAsync().Result;
                //Task.Delay(5000).Wait();
                //var bestStoryIds = _hackerNewsClient.GetBestStoriesAsync().Result;
                //Task.Delay(5000).Wait();
                var newStoryIds = _hackerNewsClient.GetNewStoriesAsync().Result;

                var orderedIds = topStoryIds
                    //.Concat(bestStoryIds)
                    .Concat(newStoryIds)
                    .Distinct()
                    .AsQueryable()
                    .OrderBy(id => id);

                var pager = new AsynchronousTaskPager<int, HackerNewsItemsResponseModel>(orderedIds, logger);
                pager.ResultSelector = id => _hackerNewsClient.GetStoryByIdAsync(id, cancellationToken);
                pager.RunAsync().Wait();
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex.ToString());
            }
        }
    }
}
