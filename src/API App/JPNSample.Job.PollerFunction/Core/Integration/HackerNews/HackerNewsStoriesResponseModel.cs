using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace JPNSample.API.Core.Integration.HackerNews
{
    public class HackerNewsStoriesResponseModel
    {
        public string By { get; set; } // Author
        public int Descendants { get; set; }
        public int Id { get; set; }
        public IEnumerable<int> Kids { get; set; }
        public int Score { get; set; }
        public int Time { get; set; }
        public string Title { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public HackerNewsItemTypeModel Type { get; set; }
        public string Url { get; set; }
    }
}
