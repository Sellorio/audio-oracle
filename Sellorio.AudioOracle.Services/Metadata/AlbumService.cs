using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sellorio.AudioOracle.Data;
using Sellorio.AudioOracle.Data.Metadata;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Library.Results.Messages;
using Sellorio.AudioOracle.Models.Metadata;
using Sellorio.AudioOracle.ServiceInterfaces.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Services.Metadata;

internal class AlbumService(DatabaseContext databaseContext, ILogger<AlbumService> logger, IMetadataMapper mapper) : IAlbumService
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
        return mapper.Map(data);
    }

    public async Task<Result> DeleteAlbumAsync(int id, bool deleteFiles = true)
    {
        var query = WithFields(databaseContext.Albums, AlbumFields.Tracks);
        var data = await query.SingleOrDefaultAsync(x => x.Id == id);

        if (data == null)
        {
            return ResultMessage.Error("Album not found.");
        }

        databaseContext.Albums.Remove(data);

        if (deleteFiles)
        {
            foreach (var track in data.Tracks)
            {
                track.Status = TrackStatus.DeleteRequested;
            }

            await AttemptToTracksAsync(data.Tracks);
        }
        else
        {
            databaseContext.Tracks.RemoveRange(data.Tracks);
        }

        await databaseContext.SaveChangesAsync();

        return Result.Success();
    }

    private Task AttemptToTracksAsync(IList<TrackData> data)
    {
        foreach (var track in data)
        {
            try
            {
                if (track.Filename != null && File.Exists(track.Filename))
                {
                    File.Delete(track.Filename);
                }

                databaseContext.Tracks.Remove(track);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    "Failed to delete track file \"{TrackFilename}\":\r\n{ExceptionType}:\r\n{ExceptionMessage}",
                    track.Filename,
                    ex.GetType().AssemblyQualifiedName,
                    ex.Message);
            }
        }

        return Task.CompletedTask;
    }

    private static IQueryable<AlbumData> WithFields(IQueryable<AlbumData> query, AlbumFields fields)
    {
        query = query.Include(x => x.AlbumArt);

        if (fields.HasFlag(AlbumFields.Artists))
        {
            query = query.Include(x => x.Artists);
        }

        if (fields.HasFlag(AlbumFields.Tracks))
        {
            if (fields.HasFlag(AlbumFields.Artists))
            {
                query = query.Include(x => x.Tracks).ThenInclude(x => x.Artists);
            }
            else
            {
                query = query.Include(x => x.Tracks);
            }
        }

        return query;
    }
}
