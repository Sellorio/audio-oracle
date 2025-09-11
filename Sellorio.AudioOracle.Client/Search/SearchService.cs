using Sellorio.AudioOracle.Client.Internal;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Search;
using Sellorio.AudioOracle.ServiceInterfaces.Search;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Client.Search;

internal class SearchService(IRestClient restClient) : ISearchService
{
    public async Task<ValueResult<IList<SearchResult>>> SearchAsync(string searchText)
    {
        return await restClient.Post($"/search{new { searchText }}").ToValueResult<IList<SearchResult>>();
    }
}
