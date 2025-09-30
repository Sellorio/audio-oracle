using System.Threading.Tasks;
using Sellorio.AudioOracle.Client.Internal;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.ServiceInterfaces.Metadata;

namespace Sellorio.AudioOracle.Client.Metadata;

internal class TrackService(IRestClient restClient) : ITrackService
{
    public Task<Result> RetryAllTracksAsync(int albumId)
    {
        return restClient.Post($"/albums/{albumId}/tracks/retry-all").ToResult();
    }
}
