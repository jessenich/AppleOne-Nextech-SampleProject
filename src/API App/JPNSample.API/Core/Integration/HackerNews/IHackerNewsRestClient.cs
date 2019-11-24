using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPNSample.API.Core.Integration.HackerNews
{
    public interface IHackerNewsRestClient
    {
        Task<IEnumerable<int>> GetTopStoriesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<int>> GetNewStoriesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<int>> GetBestStoriesAsync(CancellationToken cancellationToken = default);
        Task<HackerNewsItemsResponseModel> GetStoryByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
