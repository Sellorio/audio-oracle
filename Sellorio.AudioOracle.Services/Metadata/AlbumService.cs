using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sellorio.AudioOracle.Data;
using Sellorio.AudioOracle.Data.Metadata;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Library.Results.Messages;
using Sellorio.AudioOracle.Models;
using Sellorio.AudioOracle.Models.Content;
using Sellorio.AudioOracle.Models.Metadata;
using Sellorio.AudioOracle.ServiceInterfaces.Metadata;
using Sellorio.AudioOracle.Services.Content;
using Sellorio.AudioOracle.Services.TaskQueue.Queuers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Services.Metadata;

internal class AlbumService(DatabaseContext databaseContext, ILogger<AlbumService> logger, IMetadataMapper mapper, IFileService fileService, IRefreshTrackTagsTaskQueuingService refreshTrackTagsTaskQueuingService) : IAlbumService
{
    public async Task<ValueResult<PageResult<Album>>> GetAlbumPageAsync(int pageNumber, int pageSize, bool onlyAlbumsRequiringAttention = false, AlbumFields fields = AlbumFields.None)
    {
        if (pageNumber < 1)
        {
            return ResultMessage.Error("Page number must be at least 1.");
        }

        if (pageSize < 1)
        {
            return ResultMessage.Error("Page size must be at least 1.");
        }

        var query = databaseContext.Albums.AsQueryable();

        if (onlyAlbumsRequiringAttention)
        {
            query = query.Where(x =>
                x.Tracks!.Any(y =>
                    y.Status == TrackStatus.MetadataRetrievalFailed ||
                    y.Status == TrackStatus.DownloadFailed ||
                    y.Status == TrackStatus.MissingDownloadSource));
        }

        var totalCount = await query.CountAsync();
        var data =
            await WithFields(
                    query
                        .OrderBy(x => x.Title)
                        .ThenBy(x => x.Id)
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize),
                    fields)
                .AsNoTracking()
                .ToArrayAsync();

        return new PageResult<Album>
        {
            Items = data.Select(mapper.Map).ToArray(),
            TotalCount = totalCount
        };
    }

    public async Task<ValueResult<IList<Album>>> GetAlbumsAsync(AlbumFields fields = AlbumFields.None)
    {
        var query = WithFields(databaseContext.Albums, fields);
        var data = await query.AsNoTracking().OrderBy(x => x.Title).ToArrayAsync();
        return data.Select(mapper.Map).ToArray();
    }

    public async Task<ValueResult<Album>> GetAlbumAsync(int id, AlbumFields fields = AlbumFields.None)
    {
        var query = WithFields(databaseContext.Albums, fields);
        var data = await query.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);

        if (data == null)
        {
            return ResultMessage.NotFound("Album");
        }

        var model = mapper.Map(data);

        return model;
    }

    public async Task<Result> DeleteAlbumAsync(int id, bool deleteFiles = true)
    {
        return await databaseContext.WithTransaction(async transaction =>
        {
            var query = WithFields(databaseContext.Albums, AlbumFields.Tracks | AlbumFields.Artists);
            var data = await query.SingleOrDefaultAsync(x => x.Id == id);

            if (data == null)
            {
                return ResultMessage.NotFound("Album");
            }

            databaseContext.Albums.Remove(data);
            databaseContext.AlbumArtists.RemoveRange(data.Artists!);
            databaseContext.Tracks.RemoveRange(data.Tracks!);
            databaseContext.TrackArtists.RemoveRange(data.Tracks!.SelectMany(x => x.Artists!));

            if (deleteFiles)
            {
                try
                {
                    Directory.Delete(Path.Combine("/music", data.FolderName), true);
                }
                catch (Exception ex)
                {
                    logger.LogException(ex, "Failed to delete album files.");
                }
            }

            await databaseContext.SaveChangesAsync();
            await fileService.DeleteAsync(data.AlbumArtId!.Value);

            return Result.Success();
        });
    }

    public async Task<ValueResult<Album>> UpdateAlbumArtAsync(int id, FileType imageType, Stream stream)
    {
        if (imageType is not FileType.ImagePng and not FileType.ImageJpeg)
        {
            return ResultMessage.Error("Can only be a PNG or JPG file.");
        }

        var result = await databaseContext.WithTransaction<ValueResult<Album>>(async transaction =>
        {
            var albumData = await databaseContext.Albums.FindAsync(id);

            if (albumData == null)
            {
                return ResultMessage.NotFound("Album");
            }

            var fileCreateResult = await fileService.CreateFileAsync(imageType, stream);

            if (!fileCreateResult.WasSuccess)
            {
                return ValueResult<Album>.Failure(fileCreateResult.Messages);
            }

            albumData.AlbumArtId = fileCreateResult.Value.Id;

            await databaseContext.SaveChangesAsync();

            var album = mapper.Map(albumData);
            album.AlbumArt = fileCreateResult.Value;

            return album;
        });

        if (result.WasSuccess)
        {
            var tracks = await databaseContext.Tracks.Where(x => x.AlbumId == id && x.Filename != null).ToArrayAsync();

            foreach (var track in tracks)
            {
                await refreshTrackTagsTaskQueuingService.QueueAsync(track.Id);
            }
        }

        return result;
    }

    private static IQueryable<AlbumData> WithFields(IQueryable<AlbumData> query, AlbumFields fields)
    {
        query = query.Include(x => x.AlbumArt);

        if (fields.HasFlag(AlbumFields.Artists))
        {
            query = query.Include(x => x.Artists)!.ThenInclude(x => x.Artist);
        }

        if (fields.HasFlag(AlbumFields.Tracks))
        {
            query = query.Include(x => x.Tracks)!.ThenInclude(x => x.AlbumArtOverride);

            if (fields.HasFlag(AlbumFields.Artists))
            {
                query = query.Include(x => x.Tracks)!.ThenInclude(x => x.Artists)!.ThenInclude(x => x.Artist);
            }
        }

        return query;
    }
}
