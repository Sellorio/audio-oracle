using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;

namespace Sellorio.AudioOracle.Providers.YouTube.Services;

internal class TrackIdsResolver(IApiService apiService) : ITrackIdsResolver
{
    public async Task<ValueResult<string>> GetLatestIdAsync(string videoId)
    {
        var playerApiResult = await apiService.PostWithContextAsync("/player?prettyPrint=false", new { videoId });

        var probabilityStatus = playerApiResult["playabilityStatus"]!;
        var isUnplayable = probabilityStatus.Get<string>("status") is "UNPLAYABLE" or "ERROR";
        var unplayableReason = isUnplayable ? probabilityStatus.Get<string>("reason") : null;

        if (isUnplayable &&
            unplayableReason != null &&
                (unplayableReason == "This video is not available" ||
                unplayableReason.StartsWith("This video is no longer available due to a copyright claim by")))
        {
            var nextApiResult =
                await apiService.PostWithContextAsync(
                    "/next?prettyPrint=false",
                    new { enablePersistentPlaylistPanel = false, isAudioOnly = true, @params = "8gEAmgMDCNgE", playerParams = "igMDCNgE", videoId });

            var newVideoId = nextApiResult["currentVideoEndpoint"]!["watchEndpoint"]?.Get<string>("videoId")!;

            return newVideoId;
        }

        return videoId;
    }
}
