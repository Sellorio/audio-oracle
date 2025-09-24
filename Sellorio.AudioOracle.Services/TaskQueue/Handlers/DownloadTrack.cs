using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sellorio.AudioOracle.Data;
using Sellorio.AudioOracle.Library.Results;
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
        var filename = Path.Combine("/music", trackData.Album!.FolderName, trackFileName);
        var resolvedIds = new ResolvedIds { SourceId = trackData.DownloadSourceId!, SourceUrlId = trackData.DownloadSourceUrlId! };

        Result downloadResult;

        try
        {
            downloadResult = await providerInvocationService.InvokeAsync<IDownloadProvider>(trackData.DownloadSource!, x => x.DownloadTrackAsync(resolvedIds, filename));
        }
        catch (Exception ex)
        {
            if (File.Exists(filename))
            {
                try
                {
                    File.Delete(filename);
                }
                catch
                {
                }
            }

            trackData.Status = TrackStatus.DownloadFailed;
            trackData.StatusText = ex.Message;
            await databaseContext.SaveChangesAsync();
            return;
        }

        if (!downloadResult.WasSuccess)
        {
            trackData.Status = TrackStatus.DownloadFailed;
            trackData.StatusText = string.Join(" ", downloadResult.Messages.Select(x => x.Text));
            await databaseContext.SaveChangesAsync();
            return;
        }

        trackData.Filename = filename;
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
