using System.Collections.Generic;

namespace JPNSample.API.Core.Integration.HackerNews
{
    public class HackerNewsStoryIdsModel
    {
        public string Type { get; set; }
        public IEnumerable<int> Ids { get; set; }
    }
}
