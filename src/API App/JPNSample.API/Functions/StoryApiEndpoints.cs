using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.Http;
using JPNSample.API.Core;
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
            try
            {
                var idKeys = new List<string>() {
                    CacheKeys.NewStoriesCacheKey,
                    CacheKeys.TopStoriesCacheKey,
                    CacheKeys.BestStoriesCacheKey
                };

                dynamic getStoryIdGroup(IEnumerable<CacheItem<IEnumerable<int>>> caches, string storyType)
                {
                    var cache = caches.FirstOrDefault(x => x.ExtendedProperties[CacheExtendedPropertiesConstants.StoryTypeKey] == storyType);
                    return new {
                        key = cache.ExtendedProperties[CacheExtendedPropertiesConstants.StoryTypeKey],
                        count = cache.Item.Count(),
                        items = cache.Item
                    };
                }

                var cacheResults = await _cache.GetManyAsync<IEnumerable<int>>(idKeys);
                var topResults = getStoryIdGroup(cacheResults, CacheExtendedPropertiesConstants.TopStoryTypeValue);
                var newResults = getStoryIdGroup(cacheResults, CacheExtendedPropertiesConstants.NewStoryTypeValue);
                var bestResults = getStoryIdGroup(cacheResults, CacheExtendedPropertiesConstants.BestStoryTypeValue);

                this.AddCorsHeader(req);
                return new OkObjectResult(new {
                    topStories = topResults,
                    newStories = newResults,
                    bestStories = bestResults
                });
            }
            catch (Exception ex)
            {
                return new NotFoundObjectResult(ex.ToString());
            }
        }

        [FunctionName("GetStories")]
        public async Task<IActionResult> GetStories(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "stories")] HttpRequest req,
            ILogger logger)
        {
            try
            {
                var take = int.Parse(req.Query["take"].FirstOrDefault() ?? "10");
                var page = int.Parse(req.Query["page"].FirstOrDefault() ?? "1");

                var requestBody = await req.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(requestBody))
                    return new StatusCodeResult(400);

                var ids = JsonConvert.DeserializeObject<IEnumerable<int>>(requestBody);
                if (ids == null || ids.Count() == 0)
                    return new NoContentResult();

                var keys = ids.Select(id => string.Format(CacheKeys.StoryContentCacheKeyFormat, id.ToString()));

                var stories = await _cache.GetManyAsync<HackerNewsStoriesResponseModel>(keys);

                if (stories.Count() <= ((page - 1) * take))
                    page = 1;

                var storiesResponse = stories
                .Select(cache => cache.Item)
                .Where(story => story.Url != null)
                .OrderByDescending(story => story.Id)
                .Skip((page - 1) * take)
                .Take(take)
                .Select(story => new {
                    id = story.Id,
                    type = story.Type.GetEnumMemberStringValue(),
                    author = story.By,
                    title = story.Title,
                    url = story.Url,
                    score = story.Score,
                    createdAt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(story.Time),
                });

                this.AddCorsHeader(req);
                return new OkObjectResult(storiesResponse);
            }
            catch (Exception ex)
            {
                return new NotFoundObjectResult(ex.ToString());
            }
        }

        private bool AddCorsHeader(HttpRequest request)
        {
            return request.HttpContext.Response.Headers.TryAdd("Access-Control-Allow-Origin", "*");
        }
    }
}
