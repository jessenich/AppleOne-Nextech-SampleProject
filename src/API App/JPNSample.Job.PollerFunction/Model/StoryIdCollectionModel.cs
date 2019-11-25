using System.Collections.Generic;

namespace JPNSample.API.Model
{
    public class StoryIdCollectionModel
    {
        public int Count { get; set; }
        public string Type { get; set; }
        public IEnumerable<int> Ids { get; set; }
    }
}
