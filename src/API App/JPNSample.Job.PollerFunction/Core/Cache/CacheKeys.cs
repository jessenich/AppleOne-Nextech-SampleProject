using System;
using System.Collections.Generic;
using System.Text;

namespace JPNSample.API.Core.Cache
{
    public static class CacheKeys
    {
        public const string TopStoriesCacheKey = "topStories";
        public const string BestStoriesCacheKey = "bestStories";
        public const string NewStoriesCacheKey = "newStories";
        public const string StoryContentCacheKeyFormat = "story:{0}";
    }
}
