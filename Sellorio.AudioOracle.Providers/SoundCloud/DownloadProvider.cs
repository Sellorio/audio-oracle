using System;
using System.IO;
using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Library.Results.Messages;
using Sellorio.AudioOracle.Providers.Models;
using Sellorio.AudioOracle.Providers.SoundCloud.Services;

namespace Sellorio.AudioOracle.Providers.SoundCloud;

internal partial class DownloadProvider(ISoundCloudApiService soundCloudApiService) : IDownloadProvider
{
    public string ProviderName => Constants.ProviderName;

    public async Task<Result> DownloadTrackAsync(ResolvedIds trackIds, string outputFilename)
    {
        var track =
            await soundCloudApiService.GetTrackAsync(long.Parse(trackIds.SourceId))
                ?? throw new InvalidOperationException("This wouldn't have happened if you resolved ids first!");

        try
        {
            await soundCloudApiService.DownloadTrackAsync(track, outputFilename);
        }
        catch
        {
            if (File.Exists(outputFilename))
            {
                File.Delete(outputFilename);
            }

            throw;
        }

        return Result.Success();
    }

    public bool IsSupportedDownloadUrl(string url)
    {
        return Constants.SoundCloudUrlRegex().IsMatch(url);
    }

    public async Task<ValueResult<ResolvedIds>> ResolveIdsFromTrackUrlAsync(string downloadUrl)
    {
        var track = await soundCloudApiService.GetTrackAsync(downloadUrl);

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
