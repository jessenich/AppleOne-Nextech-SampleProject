using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using JPNSample.API.Core.Cache;
using JPNSample.API.Core.Integration.Extensions.Flurl;
using Microsoft.Extensions.Logging;

namespace JPNSample.API.Core.Integration.HackerNews
{
    public class HackerNewsRestClient : IHackerNewsRestClient
    {
        private readonly Url _baseFlurlUrl;
        private readonly ICacheProvider _cache;
        private readonly ILogger _logger;

        public HackerNewsRestClient(string baseUrl, ICacheProvider cacheProvider, ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl));

            // Append first path segment to HN API
            _baseFlurlUrl = baseUrl
                .ForceHttps()
                .AppendPathSegment("v0")
                .SetQueryParams(new {
                    print = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONENT")?.ToLower() ==
                        "production" ? "none" : "pretty"
                });

            _cache = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<HackerNewsStoriesResponseModel> GetStoryByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            // Try checking for existing cached items
            // Return cached item if found
            var cacheKey = string.Format(CacheKeys.StoryContentCacheKeyFormat, id.ToString());
            var cachedValue = await _cache.GetAsync<HackerNewsStoriesResponseModel>(cacheKey);
            if (cachedValue != null)
                return cachedValue.Item;


            // Build Top Stories Flurl URL
            var url = _baseFlurlUrl
                .Clone()
                .AppendPathSegments("item", $"{id.ToString()}.json")
                .ThrowIfNotValidUrl(); // Throw FormatException if URL somehow became malformed

            // Asynchronously get response and deserialize
            var response = await url.GetJsonAsync<HackerNewsStoriesResponseModel>(
                cancellationToken,
                HttpCompletionOption.ResponseContentRead); // No need to read headers to consider response completion

            var cacheItem = new CacheItem<HackerNewsStoriesResponseModel>(response, DateTime.UtcNow.AddDays(1));

            // Set cached response to in-memory cache
            await _cache?.SetAsync(cacheKey, cacheItem);
            return response;
        }

        public async Task<HackerNewsStoryIdsModel> GetTopStoriesAsync(CancellationToken cancellationToken = default)
        {
            // Try checking for existing cached items, return cached item if found
            var cachedValue = await _cache.GetAsync<List<int>>(CacheKeys.TopStoriesCacheKey);
            if (cachedValue != null)
                return this.CreateResult(CacheExtendedPropertiesConstants.TopStoryTypeValue, cachedValue.Item);

            // Build Top Stories Flurl URL
            var url = _baseFlurlUrl
                .Clone()
                .AppendPathSegment("topstories.json")
                .ThrowIfNotValidUrl(); // Throw FormatException if URL somehow became malformed

            // Asynchronously get response and deserialize
            var response = await url.GetJsonAsync<List<int>>(
                cancellationToken: cancellationToken,
                completionOption: HttpCompletionOption.ResponseContentRead);

            // Instantiate new result object
            var result = this.CreateResult(CacheExtendedPropertiesConstants.NewStoryTypeValue, response);

            // if cache instance provided, try set new cache
            var cacheItem = new CacheItem<List<int>>(response, DateTime.UtcNow.AddMinutes(60));

            // Add stories for future expansion with minimal data migration
            cacheItem.ExtendedProperties.Add(CacheExtendedPropertiesConstants.UrlSourceKey, url.ToString());
            cacheItem.ExtendedProperties.Add(CacheExtendedPropertiesConstants.StoryTypeKey, CacheExtendedPropertiesConstants.TopStoryTypeValue);
            await _cache?.SetAsync(CacheKeys.TopStoriesCacheKey, cacheItem);

            return result;
        }

        public async Task<HackerNewsStoryIdsModel> GetNewStoriesAsync(CancellationToken cancellationToken = default)
        {
            // Try checking for existing cached items, return cached item if found
            var cachedValue = await _cache.GetAsync<List<int>>(CacheKeys.NewStoriesCacheKey);
            if (cachedValue != null)
                return this.CreateResult(CacheExtendedPropertiesConstants.NewStoryTypeValue, cachedValue.Item);

            // Build Top Stories Flurl URL
            var url = _baseFlurlUrl
                .Clone()
                .AppendPathSegment("newstories.json")
                .ThrowIfNotValidUrl(); // Throw FormatException if URL somehow became malformed

            // Asynchronously get response and deserialize
            // Included explicit variable names for quick readability, 
            // espescially for those not familiar with Flurl.Http
            var response = await url.GetJsonAsync<List<int>>(
                cancellationToken: cancellationToken,
                completionOption: HttpCompletionOption.ResponseContentRead);

            // Instantiate new result object
            var result = this.CreateResult(CacheExtendedPropertiesConstants.NewStoryTypeValue, response);

            // if cache instance provided, try set new cache
            var cacheItem = new CacheItem<List<int>>(response, DateTime.UtcNow.AddMinutes(60));

            // Add stories for future expansion with minimal data migration
            cacheItem.ExtendedProperties.Add(CacheExtendedPropertiesConstants.UrlSourceKey, url.ToString());
            cacheItem.ExtendedProperties.Add(CacheExtendedPropertiesConstants.UrlSourceKey, CacheExtendedPropertiesConstants.NewStoryTypeValue);
            await _cache?.SetAsync(CacheKeys.NewStoriesCacheKey, cacheItem);

            return result;
        }
        public async Task<HackerNewsStoryIdsModel> GetBestStoriesAsync(CancellationToken cancellationToken = default)
        {
            // Try checking for existing cached items, return cached item if found
            var cachedValue = await _cache.GetAsync<List<int>>(CacheKeys.BestStoriesCacheKey);
            if (cachedValue != null)
                return this.CreateResult(CacheExtendedPropertiesConstants.BestStoryTypeValue, cachedValue.Item);

            // Build Top Stories Flurl URL
            var url = _baseFlurlUrl
                .Clone()
                .AppendPathSegment("beststories.json")
                .ThrowIfNotValidUrl(); // Throw FormatException if URL somehow became malformed

            // Asynchronously get response and deserialize
            // Included explicit variable names for quick readability, 
            // espescially for those not familiar with Flurl.Http
            var response = await url.GetJsonAsync<List<int>>(
                cancellationToken: cancellationToken,
                completionOption: HttpCompletionOption.ResponseContentRead);

            // Instantiate new result object
            var result = this.CreateResult(CacheExtendedPropertiesConstants.BestStoryTypeValue, response);

            // if cache instance provided, try set new cache
            var cacheItem = new CacheItem<List<int>>(response, DateTime.UtcNow.AddMinutes(60));

            // Add stories for future expansion with minimal data migration
            cacheItem.ExtendedProperties.Add("urlSource", url.ToString());
            cacheItem.ExtendedProperties.Add("type", CacheExtendedPropertiesConstants.BestStoryTypeValue);
            await _cache?.SetAsync(CacheKeys.BestStoriesCacheKey, cacheItem);

            return result;
        }

        private HackerNewsStoryIdsModel CreateResult(string type, IEnumerable<int> ids) => new HackerNewsStoryIdsModel() {
            Type = type,
            Ids = ids
        };
    }
}
