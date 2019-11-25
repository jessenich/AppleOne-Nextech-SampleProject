using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using JPNSample.API.Core.Cache;
using JPNSample.API.Core.Integration.HackerNews;
using JPNSample.API.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JPNSample.API.Functions
{
    public class StoryApiEndpoints
    {
        private readonly ICacheProvider _cache;

        public StoryApiEndpoints(ICacheProvider cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        [FunctionName("GetStoriesTypes")]
        public async Task<IActionResult> GetStoryTypes(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "types")] HttpRequest req,
            ILogger logger)
        {
            var idKeys = new List<string>() {
                CacheKeys.NewStoriesCacheKey,
                CacheKeys.TopStoriesCacheKey
            };

            IEnumerable<dynamic> getGroupedAndSplitResults(IEnumerable<CacheItem<IEnumerable<int>>> caches, string groupingKey)
            {
                caches = caches.Where(x => x.ExtendedProperties[CacheExtendedPropertiesConstants.StoryTypeKey] == groupingKey);
                return caches.Select(x => new {
                        key = x.ExtendedProperties[CacheExtendedPropertiesConstants.StoryTypeKey],
                        count = x.Item.Count(),
                        items = x.Item.Select(item => item)
                });
            }

            var cacheResults = await _cache.GetManyAsync<IEnumerable<int>>(idKeys);
            var topResults = getGroupedAndSplitResults(cacheResults, CacheExtendedPropertiesConstants.TopStoryTypeValue).FirstOrDefault();
            var newResults = getGroupedAndSplitResults(cacheResults, CacheExtendedPropertiesConstants.NewStoryTypeValue).FirstOrDefault();

            return new OkObjectResult(new {
                top = topResults,
                @new = newResults
            });
        }

        [FunctionName("GetStories")]
        public async Task<IActionResult> GetStories(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "stories")] HttpRequest req,
            ILogger logger)
        {
            var take = int.Parse(req.Query["take"].FirstOrDefault() ?? "10");
            var page = int.Parse(req.Query["page"].FirstOrDefault() ?? "1");

            var ids = JsonConvert.DeserializeObject<IEnumerable<int>>(await req.ReadAsStringAsync());
            var keys = ids.Select(id => string.Format(CacheKeys.StoryContentCacheKeyFormat, id.ToString()));

            var stories = await _cache.GetManyAsync<HackerNewsItemsResponseModel>(keys);

            if (stories.Count() <= ((page - 1) * take))
                page = 1;

            var storiesResponse = stories
                .Select(story => story.Item)
                .Where(story => story.Url != null)
                .OrderByDescending(story => story.Id)
                .Skip((page - 1) * take)
                .Take(take)
                .Select(story => new {
                    author = story.By,
                    title = story.Title,
                    url = story.Url,
                    score = story.Score,
                    createdAt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(story.Time),
                });

            return new OkObjectResult(storiesResponse);
        }
    }
}
