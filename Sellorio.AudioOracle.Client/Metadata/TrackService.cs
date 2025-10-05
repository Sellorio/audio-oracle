using System.Threading.Tasks;
using Sellorio.AudioOracle.Client.Internal;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Metadata;
using Sellorio.AudioOracle.ServiceInterfaces.Metadata;

namespace Sellorio.AudioOracle.Client.Metadata;

internal class TrackService(IRestClient restClient) : ITrackService
{
    public Task<ValueResult<Track>> ChangeDownloadSourceAsync(int albumId, int trackId, DownloadSource downloadSource, bool redownloadTrack)
    {
        return restClient.Put($"/albums/{albumId}/tracks/{trackId}/download-source{new { redownloadTrack }}", downloadSource).ToValueResult<Track>();
    }

    public Task<ValueResult<Track>> RequestTrackAsync(int albumId, int trackId)
    {
        return restClient.Post($"/albums/{albumId}/tracks/{trackId}/request").ToValueResult<Track>();
    }

    public Task<Result> RetryAllTracksAsync(int albumId)
    {
        return restClient.Post($"/albums/{albumId}/tracks/retry-all").ToResult();
    }

    public Task<ValueResult<Track>> UnrequestTrackAsync(int albumId, int trackId, bool deleteFile)
    {
        return restClient.Post($"/albums/{albumId}/tracks/{trackId}/unrequest{new { deleteFile }}").ToValueResult<Track>();
    }
}
