using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace JPNSample.API.Core.Cache
{
    public interface ICacheProvider
    {
        Task<CacheItem<TItem>> GetAsync<TItem>(string key) 
            where TItem : class;

        Task<IEnumerable<CacheItem<TItem>>> GetManyAsync<TItem>(IEnumerable<string> keys)
            where TItem : class;

        Task<CacheItem<TItem>> SetAsync<TItem>(string key, CacheItem<TItem> cacheItem) 
            where TItem : class;

        Task<CacheItem<TItem>> RemoveAsync<TItem>(string key)
            where TItem : class;
    }
}