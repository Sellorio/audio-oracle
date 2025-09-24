using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Metadata;

namespace Sellorio.AudioOracle.ServiceInterfaces.Metadata;

public interface IAlbumCreationService
{
    Task<ValueResult<Album>> CreateAlbumAsync(AlbumPost albumPost);
}