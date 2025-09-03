using System.Threading.Tasks;
using Sellorio.AudioOracle.Client.Internal;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Metadata;
using Sellorio.AudioOracle.Models.Search;
using Sellorio.AudioOracle.ServiceInterfaces.Metadata;

namespace Sellorio.AudioOracle.Client.Metadata;

internal class AlbumCreationService(IRestClient restClient) : IAlbumCreationService
{
    public async Task<ValueResult<Album>> CreateAlbumFromSearchResultAsync(SearchResult searchResult)
    {
        return await restClient.Post($"/api/albums/from-search", searchResult).ToValueResult<Album>();
    }
}
