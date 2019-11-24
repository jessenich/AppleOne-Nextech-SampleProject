using System.Runtime.Serialization;

namespace JPNSample.API.Core.Integration.HackerNews
{
    public enum HackerNewsItemTypeModel
    {
        [EnumMember(Value = "job")]
        Job,

        [EnumMember(Value = "story")]
        Story,

        [EnumMember(Value = "comment")]
        Comment,

        [EnumMember(Value = "poll")]
        Poll,

        [EnumMember(Value = "pollopt")]
        PollOpt
    }
}
