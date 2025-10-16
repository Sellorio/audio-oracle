using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Providers.Common;
using Sellorio.AudioOracle.Providers.Models;
using Sellorio.AudioOracle.Providers.YouTube.Services;

namespace Sellorio.AudioOracle.Providers.YouTube;

internal partial class DownloadProvider(ITrackIdsResolver trackIdsResolver, IYtDlpService ytDlpService, IFfmpegService ffmpegService) : IDownloadProvider
{
    public string ProviderName => Constants.ProviderName;

    public bool IsSupportedDownloadUrl(string url)
    {
        return Constants.TrackUriRegex().IsMatch(url);
    }

    public async Task<ValueResult<ResolvedIds>> ResolveIdsFromTrackUrlAsync(string downloadUrl)
    {
        var urlId = Constants.TrackUriRegex().Match(downloadUrl).Groups[1].Value;
        var latestIdResult = await trackIdsResolver.GetLatestIdAsync(urlId);

        if (!latestIdResult.WasSuccess)
        {
            return ValueResult<ResolvedIds>.Failure(latestIdResult.Messages);
        }

        return new ResolvedIds { SourceId = latestIdResult.Value, SourceUrlId = latestIdResult.Value };
    }

    public async Task<Result> DownloadTrackAsync(ResolvedIds trackIds, string outputFilename)
    {
        var tempOutputDir = Path.Combine(Path.GetTempPath(), trackIds.SourceId);

        Directory.CreateDirectory(tempOutputDir);

        var result =
            await ytDlpService.InvokeYtDlpAsync(
                $"--cookies {Constants.CookiesPath} --ffmpeg-location {Constants.FfmpegPath} \"https://music.youtube.com/watch?v={trackIds.SourceId}\"",
                tempOutputDir);

        if (!result.WasSuccess)
        {
            Directory.Delete(tempOutputDir, true);
            return result;
        }

        var mediaFilename = Directory.GetFiles(tempOutputDir).Single();

        result = await ffmpegService.ConvertToMp3Async(mediaFilename, outputFilename, outputBitrateKbps: 256, loudnessNormalization: true);
        Directory.Delete(tempOutputDir, true);
        return result;
    }
}
