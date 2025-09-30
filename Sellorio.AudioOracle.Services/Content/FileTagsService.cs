using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sellorio.AudioOracle.Data;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Library.Results.Messages;
using Sellorio.AudioOracle.Models.Metadata;
using TagLib;

namespace Sellorio.AudioOracle.Services.Content;

internal class FileTagsService(ILogger<FileTagsService> logger, DatabaseContext databaseContext) : IFileTagsService
{
    public async Task<Result> UpdateFileTagsAsync(string filename, Album album, Track track)
    {
        try
        {
            var albumArtBytes =
            await databaseContext.FileInfos
                .AsNoTracking()
                .Where(x => x.Id == album.AlbumArt.Id)
                .Select(x => x.Content!.Data)
                .SingleAsync();

            using var mp3 = File.Create(filename);
            var tag = mp3.GetTag(TagTypes.Id3v2, true);

            tag.Title = track.Title + (track.AlternateTitle == null ? string.Empty : $" - {track.AlternateTitle}");
            tag.Album = album.Title;
            tag.AlbumArtists = album.Artists.Select(x => x.Name).ToArray();
            tag.Performers = track.Artists.Select(x => x.Name).ToArray();
            tag.Year = album.ReleaseYear ?? default;
            tag.Track = (uint)track.TrackNumber!;
            tag.TrackCount = album.TrackCount;

            var albumArtPicture = new Picture
            {
                Type = PictureType.FrontCover,
                MimeType = album.AlbumArt.MimeType,
                Data = new ByteVector(albumArtBytes)
            };

            tag.Pictures = []; // i was recommended to explicitly clear the list as a separate step
            tag.Pictures = [albumArtPicture];

            mp3.Save();
        }
        catch (Exception ex)
        {
            logger.LogException(ex, "Failed to update file tags.");
            return ResultMessage.Error("Failed to update file tags: " + ex.Message);
        }

        return Result.Success();
    }
}
