using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace JPNSample.API.Core.Cache
{
    public class RedisCacheProvider : ICacheProvider
    {
        private readonly IDatabaseAsync _redisStore;
        private readonly ILogger _logger;

        public RedisCacheProvider(ConnectionMultiplexer connectionMultiplexer, ILogger logger)
        {
            if (connectionMultiplexer == null)
                throw new ArgumentNullException(nameof(connectionMultiplexer));
            
            _redisStore = connectionMultiplexer.GetDatabase();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CacheItem<TItem>> GetAsync<TItem>(string key) 
            where TItem : class
        {
            // Using fail-first patterns, check valid params supplied prior to any futher processing
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            
            // Try to find cached value using key
            var value = await _redisStore.StringGetAsync(key);
            
            // Return null when no cached item found
            if (value.IsNullOrEmpty)
                return null;

            var item = JsonConvert.DeserializeObject<CacheItem<TItem>>(value);

            if (item.UtcDateExpired < DateTime.UtcNow)
                return null;
            else
                return item;
        }

        public async Task<IEnumerable<CacheItem<TItem>>> GetManyAsync<TItem>(IEnumerable<string> keys)
            where TItem : class
        {
            if (keys == null || keys.Count() == 0)
                throw new ArgumentNullException(nameof(keys));

            var results = await _redisStore.StringGetAsync(keys.Select(key => (RedisKey)key).ToArray());
            if (results == null || results.Length == 0)
                return null;

            return results
                .Where(value => !value.IsNullOrEmpty)
                .Select(value => JsonConvert.DeserializeObject<CacheItem<TItem>>(value));
        }

        public async Task<CacheItem<TItem>> SetAsync<TItem>(string key, CacheItem<TItem> cacheItem) 
            where TItem : class
        {
            // Using fail-first patterns, check valid params supplied prior to any futher processing
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            if (cacheItem == null)
                throw new ArgumentNullException(nameof(cacheItem));

            var jsonCacheItem = JsonConvert.SerializeObject(cacheItem);
            await _redisStore.StringSetAsync(key, jsonCacheItem);
            return cacheItem;
        }

        public async Task<CacheItem<TItem>> RemoveAsync<TItem>(string key) 
            where TItem : class
        {
            // Using fail-first patterns, check valid params supplied prior to any futher processing
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            var memItem = await this.GetAsync<TItem>(key);
            await _redisStore.KeyDeleteAsync(key);
            return memItem;
        }
    }
}
