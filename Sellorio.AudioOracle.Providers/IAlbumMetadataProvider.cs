using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Providers.Models;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Providers;

public interface IAlbumMetadataProvider : IProvider
{
    Task<ValueResult<ResolvedIds>> ResolveAlbumIdsAsync(string sourceUrlId);
    Task<ValueResult<AlbumMetadata>> GetAlbumMetadataAsync(ResolvedIds resolvedIds);
}
