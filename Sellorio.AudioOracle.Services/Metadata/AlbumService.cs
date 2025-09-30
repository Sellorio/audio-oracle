using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sellorio.AudioOracle.Data;
using Sellorio.AudioOracle.Data.Metadata;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Library.Results.Messages;
using Sellorio.AudioOracle.Models.Metadata;
using Sellorio.AudioOracle.ServiceInterfaces.Metadata;
using Sellorio.AudioOracle.Services.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Services.Metadata;

internal class AlbumService(DatabaseContext databaseContext, ILogger<AlbumService> logger, IMetadataMapper mapper, IFileService fileService) : IAlbumService
{
    public async Task<ValueResult<IList<Album>>> GetAlbumsAsync(AlbumFields fields = AlbumFields.None)
    {
        var query = WithFields(databaseContext.Albums, fields);
        var data = await query.AsNoTracking().ToArrayAsync();
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
