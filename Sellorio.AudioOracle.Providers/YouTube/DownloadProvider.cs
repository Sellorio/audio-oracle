using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
        return YouTubeTrackUriRegex().IsMatch(url);
    }

    public async Task<ValueResult<ResolvedIds>> ResolveIdsFromTrackUrlAsync(string downloadUrl)
    {
        var urlId = YouTubeTrackUriRegex().Match(downloadUrl).Groups[1].Value;
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
                $"--cookies {Constants.CookiesPath} --ffmpeg-location {Constants.FfmpegPath} \"https://music.youtube.com/watch?v={trackIds.SourceUrlId}\"",
                tempOutputDir);

        if (!result.WasSuccess)
        {
            Directory.Delete(tempOutputDir, true);
            return result;
        }

        var mediaFilename = Directory.GetFiles(tempOutputDir).Single();

        result = await ffmpegService.ConvertToMp3Async(mediaFilename, outputFilename, outputBitrateKbps: 256, loudnessNormalization: false);
        Directory.Delete(tempOutputDir, true);
        return result;
    }

    // Supported Uri Formats:
    // https://music.youtube.com/watch?v=Cqp-dB7GVI8&list=PLIr8oAMYGij0QrgUfzLyqbwrHfaBtXL1w
    // https://youtu.be/Cqp-dB7GVI8
    // https://www.youtube.com/watch?v=Cqp-dB7GVI8
    [GeneratedRegex(@"^https:\/\/(?:music\.youtube\.com\/watch\?v=|youtu\.be\/|www\.youtube\.com\/watch\?v=)([a-zA-Z0-9_-]+)[&a-zA-Z0-9=_]*$", RegexOptions.IgnoreCase)]
    private static partial Regex YouTubeTrackUriRegex();
}
