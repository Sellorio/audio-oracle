using Sellorio.AudioOracle.Client.Internal;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Content;
using Sellorio.AudioOracle.Models.Metadata;
using Sellorio.AudioOracle.ServiceInterfaces.Metadata;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Client.Metadata;

internal class AlbumService(IRestClient restClient) : IAlbumService
{
    public async Task<Result> DeleteAlbumAsync(int id, bool deleteFiles = true)
    {
        return await restClient.Delete($"/albums/{id}{new { deleteFiles }}").ToResult();
    }

    public async Task<ValueResult<Album>> GetAlbumAsync(int id, AlbumFields fields = AlbumFields.None)
    {
        return await restClient.Get($"/albums/{id}{new { include = fields }}").ToValueResult<Album>();
    }

    public async Task<ValueResult<IList<Album>>> GetAlbumsAsync(AlbumFields fields = AlbumFields.None)
    {
        return await restClient.Get($"/albums{new { include = fields }}").ToValueResult<IList<Album>>();
    }

    public async Task<ValueResult<Album>> UpdateAlbumArtAsync(int id, FileType imageType, Stream stream)
    {
        return await restClient.Put($"/albums/{id}/art{new { imageType }}", stream).ToValueResult<Album>();
    }
}
