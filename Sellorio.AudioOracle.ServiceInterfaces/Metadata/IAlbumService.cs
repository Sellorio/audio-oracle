using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models;
using Sellorio.AudioOracle.Models.Content;
using Sellorio.AudioOracle.Models.Metadata;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.ServiceInterfaces.Metadata;

public interface IAlbumService
{
    Task<Result> DeleteAlbumAsync(int id, bool deleteFiles = true);
    Task<ValueResult<Album>> GetAlbumAsync(int id, AlbumFields fields = AlbumFields.None);
    Task<ValueResult<IList<Album>>> GetAlbumsAsync(AlbumFields fields = AlbumFields.None);
    Task<ValueResult<PageResult<Album>>> GetAlbumPageAsync(int pageNumber, int pageSize, bool onlyAlbumsRequiringAttention = false, AlbumFields fields = AlbumFields.None);
    Task<ValueResult<Album>> UpdateAlbumArtAsync(int id, FileType imageType, Stream stream);
}
