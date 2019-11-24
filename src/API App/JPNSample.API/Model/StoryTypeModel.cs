using System;
using System.Text;

namespace JPNSample.API.Model
{
    public class StoryTypeModel
    {
        public StoryIdCollectionModel Top { get; set; }
        public StoryIdCollectionModel Best { get; set; }
        public StoryIdCollectionModel New { get; set; }
    }
}
