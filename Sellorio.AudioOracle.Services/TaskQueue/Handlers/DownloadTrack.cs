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
using Sellorio.AudioOracle.Services.Content;
using Sellorio.AudioOracle.Services.Import;
using Sellorio.AudioOracle.Services.Metadata;

namespace Sellorio.AudioOracle.Services.TaskQueue.Handlers;

internal class DownloadTrack(
    DatabaseContext databaseContext,
    ILogger<DownloadTrack> logger,
    IProviderInvocationService providerInvocationService,
    IFileTagsService fileTagsService,
    IMetadataMapper metadataMapper,
    IImportService importService) : ITaskHandler
{
    public async Task HandleAsync(TaskHandlerContext context)
    {
        var trackData =
            await databaseContext.Tracks
                .Include(x => x.Artists)!.ThenInclude(x => x.Artist)
                .Include(x => x.Album).ThenInclude(x => x!.AlbumArt)
                .Include(x => x.Album).ThenInclude(x => x!.Artists)!.ThenInclude(x => x.Artist)
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

        var track = metadataMapper.Map(trackData);
        var album = metadataMapper.Map(trackData.Album!);

        var trackFileName = $"{trackData.TrackNumber} - {EscapePathItem(trackData.Title!)}.mp3";
        var downloadFilename = Path.Combine(Path.GetTempPath(), trackFileName);
        var resolvedIds = new ResolvedIds { SourceId = trackData.DownloadSourceId!, SourceUrlId = trackData.DownloadSourceUrlId! };

        if (!await importService.TryImportAsync(trackData.DownloadSourceId!, downloadFilename))
        {
            Result downloadResult;

            try
            {
                downloadResult =
                    await providerInvocationService.InvokeAsync<IDownloadProvider>(
                        trackData.DownloadSource!,
                        x => x.DownloadTrackAsync(resolvedIds, downloadFilename));
            }
            catch (Exception ex)
            {
                if (File.Exists(downloadFilename))
                {
                    try
                    {
                        File.Delete(downloadFilename);
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

                if (File.Exists(downloadFilename))
                {
                    try
                    {
                        File.Delete(downloadFilename);
                    }
                    catch
                    {
                    }
                }

                return;
            }
        }

        var tagsResult = await fileTagsService.UpdateFileTagsAsync(downloadFilename, album, track);

        if (!tagsResult.WasSuccess)
        {
            trackData.Status = TrackStatus.DownloadFailed;
            trackData.StatusText = string.Join(" ", tagsResult.Messages.Select(x => x.Text));
            await databaseContext.SaveChangesAsync();

            if (File.Exists(downloadFilename))
            {
                try
                {
                    File.Delete(downloadFilename);
                }
                catch
                {
                }
            }

            return;
        }

        var filename = Path.Combine("/music", trackData.Album!.FolderName, trackFileName);
        var outputDirectory = Path.GetDirectoryName(filename)!;

        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        File.Move(downloadFilename, filename);

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
