using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Providers.Models;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Providers;

public interface ITrackMetadataProvider : IProvider
{
    Task<ValueResult<ResolvedIds>> ResolveTrackIdsAsync(string sourceUrlId);
    Task<ValueResult<TrackMetadata>> GetTrackMetadataAsync(ResolvedIds resolvedIds);
}
