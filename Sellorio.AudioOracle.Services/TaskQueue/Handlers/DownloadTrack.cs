using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sellorio.AudioOracle.Data;
using Sellorio.AudioOracle.Models.Metadata;
using Sellorio.AudioOracle.Providers;
using Sellorio.AudioOracle.Providers.Models;

namespace Sellorio.AudioOracle.Services.TaskQueue.Handlers;

internal class DownloadTrack(
    DatabaseContext databaseContext,
    ILogger<DownloadTrack> logger,
    IProviderInvocationService providerInvocationService) : ITaskHandler
{
    public async Task HandleAsync(TaskHandlerContext context)
    {
        var trackData =
            await databaseContext.Tracks
                .Include(x => x.Album)
                .SingleOrDefaultAsync(x => x.Id == context.ObjectId!.Value);

        if (trackData == null)
        {
            logger.LogInformation("Skipped download for deleted track.");
            return;
        }

        if (trackData.Status != TrackStatus.Downloading)
        {
            logger.LogInformation("Skipped download since the track is not in {Status} status.", TrackStatus.MissingMetadata);
            return;
        }

        var trackFileName = $"{trackData.TrackNumber} - {EscapePathItem(trackData.Title!)}.mp3";
        var filename = Path.Combine(trackData.Album!.FolderName, trackFileName);
        var resolvedIds = new ResolvedIds { SourceId = trackData.DownloadSourceId!, SourceUrlId = trackData.DownloadSourceUrlId! };

        try
        {
            await providerInvocationService.InvokeAsync<IDownloadProvider>(trackData.DownloadSource!, x => x.DownloadTrackAsync(resolvedIds, filename));
        }
        catch (Exception ex)
        {
            trackData.Status = TrackStatus.DownloadFailed;
            trackData.StatusText = ex.Message;
            await databaseContext.SaveChangesAsync();
            return;
        }

        trackData.Status = TrackStatus.Imported;
        trackData.StatusText = null;
        await databaseContext.SaveChangesAsync();
    }

    private static string EscapePathItem(string value)
    {
        var invalidCharacters = Enumerable.Union(Path.GetInvalidPathChars(), Path.GetInvalidFileNameChars());
        var result = new StringBuilder(value.Length);

        foreach (char c in value)
        {
            if (!invalidCharacters.Contains(c))
            {
                result.Append(c);
            }
        }

        return result.ToString();
    }
}
