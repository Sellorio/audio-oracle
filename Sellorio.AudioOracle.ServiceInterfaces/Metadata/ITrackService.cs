using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;

namespace Sellorio.AudioOracle.ServiceInterfaces.Metadata;

public interface ITrackService
{
    Task<Result> RetryAllTracksAsync(int albumId);
}
