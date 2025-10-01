using System.Collections.Generic;
using System.Threading.Tasks;
using Sellorio.AudioOracle.Client.Internal;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Metadata;
using Sellorio.AudioOracle.Models.Search;
using Sellorio.AudioOracle.ServiceInterfaces.Search;

namespace Sellorio.AudioOracle.Client.Search;

internal class SearchService(IRestClient restClient) : ISearchService
{
    public async Task<ValueResult<IList<SearchResult>>> SearchAsync(string searchText)
    {
        return await restClient.Post($"/search{new { searchText }}").ToValueResult<IList<SearchResult>>();
    }

    public async Task<ValueResult<IList<DownloadSearchResult>>> SearchForDownloadAsync(int trackId)
    {
        return await restClient.Post($"/search/download{new { trackId }}").ToValueResult<IList<DownloadSearchResult>>();
    }

    public async Task<ValueResult<DownloadSource>> SearchForDownloadByUrlAsync(string url)
    {
        return await restClient.Post($"/search/download-url{new { url }}").ToValueResult<DownloadSource>();
    }
}
