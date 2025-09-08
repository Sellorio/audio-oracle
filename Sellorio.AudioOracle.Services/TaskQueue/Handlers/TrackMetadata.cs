using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sellorio.AudioOracle.Data;
using Sellorio.AudioOracle.Data.Metadata;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Content;
using Sellorio.AudioOracle.Models.Metadata;
using Sellorio.AudioOracle.Providers;
using Sellorio.AudioOracle.ServiceInterfaces.Metadata;
using Sellorio.AudioOracle.Services.Content;
using Sellorio.AudioOracle.Services.TaskQueue.Queuers;

namespace Sellorio.AudioOracle.Services.TaskQueue.Handlers;

internal class TrackMetadata(
    ILogger<TrackMetadata> logger,
    DatabaseContext databaseContext,
    IProviderInvocationService providerInvocationService,
    IArtistCreationService artistCreationService,
    IFileService fileService,
    IDownloadTrackTaskQueuingService downloadTrackTaskQueuing) : ITaskHandler
{
    public async Task HandleAsync(TaskHandlerContext context)
    {
        var trackData =
            await databaseContext.Tracks
                .Include(x => x.Album)
                .SingleOrDefaultAsync(x => x.Id == context.ObjectId!.Value);

        if (trackData == null)
        {
            logger.LogInformation("Skipped retrieving track metadata for deleted track.");
            return;
        }

        if (trackData.Status != TrackStatus.MissingMetadata)
        {
            logger.LogInformation("Skipped retrieving track metadata since the track is not in {Status} status.", TrackStatus.MissingMetadata);
            return;
        }

        ValueResult<Providers.Models.TrackMetadata> trackMetadataResult;

        try
        {
            trackMetadataResult =
                await providerInvocationService.InvokeAsync<ITrackMetadataProvider, Providers.Models.TrackMetadata>(
                    trackData.MetadataSource,
                    x => x.GetTrackMetadataAsync(
                        new() { SourceId = trackData.Album!.SourceId, SourceUrlId = trackData.Album.SourceUrlId },
                        new() { SourceId = trackData.MetadataSourceId, SourceUrlId = trackData.MetadataSourceUrlId }));
        }
        catch (Exception ex)
        {
            trackData.Status = TrackStatus.DownloadFailed;
            trackData.StatusText = ex.Message;
            await databaseContext.SaveChangesAsync();
            return;
        }

        if (!trackMetadataResult.WasSuccess)
        {
            trackData.StatusText = string.Join("\r\n", trackMetadataResult.Messages.Select(x => x.Text));
            trackData.Status = TrackStatus.MetadataRetrievalFailed;
            return;
        }

        var trackMetadata = trackMetadataResult.Value;

        var fileResult = trackMetadata.AlbumArtOverrideUrl == null ? null : await fileService.CreateFileFromUrlAsync(trackMetadata.AlbumArtOverrideUrl, FileType.ImageJpeg, FileType.ImagePng);

        if (fileResult != null && !fileResult.WasSuccess)
        {
            trackData.StatusText = string.Join("\r\n", fileResult.Messages.Select(x => x.Text));
            trackData.Status = TrackStatus.MetadataRetrievalFailed;
            return;
        }

        var artistsResult =
            await artistCreationService.GetOrCreateArtistsAsync(
                trackMetadata.ArtistIds
                    .Select(x => new ArtistPost { Source = trackData.MetadataSource, SourceId = x.SourceId, SourceUrlId = x.SourceUrlId })
                    .ToArray());

        if (!artistsResult.WasSuccess)
        {
            trackData.StatusText = string.Join("\r\n", artistsResult.Messages.Select(x => x.Text));
            trackData.Status = TrackStatus.MetadataRetrievalFailed;
            return;
        }

        trackData.TrackNumber = trackMetadata.TrackNumber;
        trackData.Duration = trackMetadata.Duration;
        trackData.Title = trackMetadata.Title;
        trackData.AlternateTitle = trackMetadata.AlternateTitle;
        trackData.Artists = artistsResult.Value.Where(x => x != null).Select(x => new TrackArtistData { ArtistId = x!.Id }).ToArray();

        if (fileResult != null)
        {
            trackData.AlbumArtOverrideId = fileResult.Value.Id;
        }

        trackData.StatusText = null;

        if (trackMetadata.DownloadIds != null)
        {
            trackData.DownloadSource = trackData.MetadataSource;
            trackData.DownloadSourceId = trackMetadata.DownloadIds.SourceId;
            trackData.DownloadSourceUrlId = trackMetadata.DownloadIds.SourceUrlId;
        }

        if (!trackData.IsRequested)
        {
            trackData.Status = TrackStatus.NotRequested;
        }
        else if (trackData.DownloadSource != null)
        {
            trackData.Status = TrackStatus.Downloading;
        }
        else
        {
            trackData.Status = TrackStatus.MissingDownloadSource;
        }

        await databaseContext.SaveChangesAsync();

        if (trackData.Status == TrackStatus.Downloading)
        {
            await downloadTrackTaskQueuing.QueueAsync(trackData.Id);
        }
    }
}
