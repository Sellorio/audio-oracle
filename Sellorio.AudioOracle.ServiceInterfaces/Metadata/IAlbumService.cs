using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Metadata;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.ServiceInterfaces.Metadata;

public interface IAlbumService
{
    Task<Result> DeleteAlbumAsync(int id, bool deleteFiles = true);
    Task<ValueResult<Album>> GetAlbumAsync(int id, AlbumFields fields = AlbumFields.None);
    Task<ValueResult<IList<Album>>> GetAlbumsAsync(AlbumFields fields = AlbumFields.None);
}
