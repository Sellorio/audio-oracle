using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sellorio.AudioOracle.Data;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Library.Results.Messages;
using Sellorio.AudioOracle.Models.Metadata;
using Sellorio.AudioOracle.ServiceInterfaces.Metadata;
using Sellorio.AudioOracle.Services.TaskQueue.Queuers;

namespace Sellorio.AudioOracle.Services.Metadata;

internal class TrackService(
    DatabaseContext databaseContext,
    ITrackMetadataTaskQueuingService trackMetadataTaskQueuingService,
    IDownloadTrackTaskQueuingService downloadTrackTaskQueuingService,
    IMetadataMapper metadataMapper) : ITrackService
{
    public async Task<Result> RetryAllTracksAsync(int albumId)
    {
        var tracks =
            await databaseContext.Tracks
                .Where(x => x.AlbumId == albumId)
                .Where(x => x.Status == TrackStatus.MetadataRetrievalFailed || x.Status == TrackStatus.DownloadFailed)
                .ToArrayAsync();

        foreach (var track in tracks)
        {
            if (track.Status == TrackStatus.MetadataRetrievalFailed)
            {
                track.Status = TrackStatus.MissingMetadata;
            }
            else if (track.Status == TrackStatus.DownloadFailed)
            {
                track.Status = TrackStatus.Downloading;
            }
            else
            {
                continue;
            }

            track.StatusText = null;
        }

        await databaseContext.SaveChangesAsync();

        foreach (var track in tracks)
        {
            if (track.Status == TrackStatus.MissingMetadata)
            {
                await trackMetadataTaskQueuingService.QueueAsync(track.Id);
            }
            else if (track.Status == TrackStatus.Downloading)
            {
                await downloadTrackTaskQueuingService.QueueAsync(track.Id);
            }
            else
            {
                continue;
            }
        }

        return Result.Success();
    }

    public async Task<ValueResult<Track>> UnrequestTrackAsync(int albumId, int trackId, bool deleteFile)
    {
        var data = await databaseContext.Tracks.FindAsync(trackId);

        if (data == null || data.AlbumId != albumId)
        {
            return ResultMessage.NotFound("Track");
        }

        if (!data.IsRequested)
        {
            return metadataMapper.Map(data);
        }

        switch (data.Status)
        {
            case TrackStatus.Imported:
                if (deleteFile)
                {
                    if (File.Exists(data.Filename))
                    {
                        File.Delete(data.Filename);
                    }

                    data.Filename = null;
                }

                data.IsRequested = false;
                data.Status = TrackStatus.NotRequested;
                data.StatusText = null;
                break;
            case TrackStatus.MetadataRetrievalFailed:
                data.IsRequested = false;
                break;
            case TrackStatus.DownloadFailed:
            case TrackStatus.MissingDownloadSource:
                data.IsRequested = false;
                data.Status = TrackStatus.NotRequested;
                data.StatusText = null;
                break;
            default:
                return ResultMessage.Error("Cannot unrequest a track while it is being processed.");
        }

        await databaseContext.SaveChangesAsync();

        return metadataMapper.Map(data);
    }

    public async Task<ValueResult<Track>> RequestTrackAsync(int albumId, int trackId)
    {
        var data = await databaseContext.Tracks.FindAsync(trackId);

        if (data == null || data.AlbumId != albumId)
        {
            return ResultMessage.NotFound("Track");
        }

        if (data.IsRequested)
        {
            return metadataMapper.Map(data);
        }

        switch (data.Status)
        {
            case TrackStatus.NotRequested:
                data.IsRequested = true;

                if (!string.IsNullOrEmpty(data.Filename))
                {
                    if (File.Exists(data.Filename))
                    {
                        data.Status = TrackStatus.Imported;
                        data.StatusText = null;
                        break;
                    }
                    else
                    {
                        data.Filename = null;
                    }
                }

                if (data.DownloadSource == null)
                {
                    data.Status = TrackStatus.MissingDownloadSource;
                    data.StatusText = null;
                }
                else
                {
                    data.Status = TrackStatus.Downloading;
                    data.StatusText = null;
                }

                break;
            default:
                return ResultMessage.Error("Cannot unrequest a track while it is being processed.");
        }

        await databaseContext.SaveChangesAsync();

        if (data.Status == TrackStatus.Downloading)
        {
            await downloadTrackTaskQueuingService.QueueAsync(trackId);
        }

        return metadataMapper.Map(data);
    }

    public async Task<ValueResult<Track>> ChangeDownloadSourceAsync(int albumId, int trackId, DownloadSource downloadSource, bool redownloadTrack)
    {
        var data = await databaseContext.Tracks.FindAsync(trackId);

        if (data == null || data.AlbumId != albumId)
        {
            return ResultMessage.NotFound("Track");
        }

        switch (data.Status)
        {
            case TrackStatus.Imported:
            case TrackStatus.NotRequested:
                if (redownloadTrack)
                {
                    if (File.Exists(data.Filename))
                    {
                        File.Delete(data.Filename);
                    }

                    data.Filename = null;
                    data.Status = data.IsRequested ? TrackStatus.Downloading : TrackStatus.NotRequested;
                    data.StatusText = null;
                }
                break;
            case TrackStatus.MetadataRetrievalFailed:
                break;
            case TrackStatus.DownloadFailed:
                data.Status = data.IsRequested ? TrackStatus.Downloading : TrackStatus.NotRequested;
                data.StatusText = null;
                break;
            case TrackStatus.MissingDownloadSource:
                data.Status = TrackStatus.Downloading;
                data.StatusText = null;
                break;
            default:
                return ResultMessage.Error("Cannot change download source a track while it is being processed.");
        }

        data.DownloadSource = downloadSource.Source;
        data.DownloadSourceId = downloadSource.SourceId;
        data.DownloadSourceUrlId = downloadSource.SourceUrlId;

        await databaseContext.SaveChangesAsync();

        if (redownloadTrack && data.Status == TrackStatus.Downloading)
        {
            await downloadTrackTaskQueuingService.QueueAsync(trackId);
        }

        return metadataMapper.Map(data);
    }
}
