using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPNSample.API.Core.Integration.HackerNews
{
    public interface IHackerNewsRestClient
    {
        Task<HackerNewsStoryIdsModel> GetTopStoriesAsync(CancellationToken cancellationToken = default);
        Task<HackerNewsStoryIdsModel> GetNewStoriesAsync(CancellationToken cancellationToken = default);
        Task<HackerNewsStoryIdsModel> GetBestStoriesAsync(CancellationToken cancellationToken = default);
        Task<HackerNewsStoriesResponseModel> GetStoryByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
