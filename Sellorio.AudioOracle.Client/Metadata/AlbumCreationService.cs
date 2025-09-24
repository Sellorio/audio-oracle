using System.Threading.Tasks;
using Sellorio.AudioOracle.Client.Internal;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Metadata;
using Sellorio.AudioOracle.ServiceInterfaces.Metadata;

namespace Sellorio.AudioOracle.Client.Metadata;

internal class AlbumCreationService(IRestClient restClient) : IAlbumCreationService
{
    public async Task<ValueResult<Album>> CreateAlbumAsync(AlbumPost albumPost)
    {
        return await restClient.Post($"/albums", albumPost).ToValueResult<Album>();
    }
}
