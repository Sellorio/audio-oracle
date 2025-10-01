using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Metadata;

namespace Sellorio.AudioOracle.ServiceInterfaces.Metadata;

public interface ITrackService
{
    Task<Result> RetryAllTracksAsync(int albumId);
    Task<ValueResult<Track>> RequestTrackAsync(int albumId, int trackId);
    Task<ValueResult<Track>> UnrequestTrackAsync(int albumId, int trackId, bool deleteFile);
    Task<ValueResult<Track>> ChangeDownloadSourceAsync(int albumId, int trackId, DownloadSource downloadSource, bool redownloadTrack);
}
