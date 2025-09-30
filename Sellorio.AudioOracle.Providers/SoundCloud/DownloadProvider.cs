using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Library.Results.Messages;
using Sellorio.AudioOracle.Providers.Common;
using Sellorio.AudioOracle.Providers.Models;
using SoundCloudExplode;

namespace Sellorio.AudioOracle.Providers.SoundCloud;

internal partial class DownloadProvider(SoundCloudClient soundCloudClient, IFfmpegService ffmpegService) : IDownloadProvider
{
    public string ProviderName => Constants.ProviderName;

    public async Task<Result> DownloadTrackAsync(ResolvedIds trackIds, string outputFilename)
    {
        var track =
            await soundCloudClient.Tracks.GetByIdAsync(long.Parse(trackIds.SourceId))
                ?? throw new InvalidOperationException("This wouldn't have happened if you resolved ids first!");
        var tempFilename = Path.Combine(Path.GetTempPath(), $"SC-{track.Id}.mp3");

        await soundCloudClient.DownloadAsync(track, tempFilename);

        try
        {
            await ffmpegService.ConvertToMp3Async(tempFilename, outputFilename, outputBitrateKbps: 256, loudnessNormalization: false);
        }
        finally
        {
            if (File.Exists(tempFilename))
            {
                File.Delete(tempFilename);
            }
        }

        return Result.Success();
    }

    public bool IsSupportedDownloadUrl(string url)
    {
        return Constants.SoundCloudUrlRegex().IsMatch(url);
    }

    public async Task<ValueResult<ResolvedIds>> ResolveIdsFromTrackUrlAsync(string downloadUrl)
    {
        var track = await soundCloudClient.Tracks.GetAsync(downloadUrl);

        if (track == null)
        {
            return ResultMessage.NotFound("SoundCloud Track");
        }

        return new ResolvedIds()
        {
            SourceId = track.Id.ToString(),
            SourceUrlId = Constants.SoundCloudUrlRegex().Match(downloadUrl).Groups[1].Value
        };
    }
}
