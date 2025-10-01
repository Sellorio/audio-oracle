using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sellorio.AudioOracle.Data;
using Sellorio.AudioOracle.Data.Metadata;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Library.Results.Messages;
using Sellorio.AudioOracle.Models.Metadata;
using Sellorio.AudioOracle.Providers;
using Sellorio.AudioOracle.Providers.Models;
using Sellorio.AudioOracle.ServiceInterfaces.Metadata;
using Sellorio.AudioOracle.Services.Content;
using Sellorio.AudioOracle.Services.TaskQueue.Queuers;

namespace Sellorio.AudioOracle.Services.Metadata;

internal class AlbumCreationService(
    DatabaseContext databaseContext,
    IMetadataMapper metadataMapper,
    IProviderInvocationService providerInvocationService,
    IFileService fileService,
    IArtistCreationService artistCreationService,
    ITrackMetadataTaskQueuingService trackMetadataTaskQueuingService) : IAlbumCreationService
{
    public async Task<ValueResult<Album>> CreateAlbumAsync(AlbumPost albumPost)
    {
        var existingAlbum =
            await databaseContext.Albums
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Source == albumPost.SearchResult.Source && x.SourceId == albumPost.SearchResult.AlbumId);

        if (existingAlbum != null)
        {
            return metadataMapper.Map(existingAlbum);
        }

        var albumMetadataResult =
            await providerInvocationService.InvokeAsync<IAlbumMetadataProvider, AlbumMetadata>(
                albumPost.SearchResult.Source,
                x => x.GetAlbumMetadataAsync(new() { SourceId = albumPost.SearchResult.AlbumId, SourceUrlId = albumPost.SearchResult.AlbumUrlId }));

        if (!albumMetadataResult.WasSuccess)
        {
            return ValueResult<Album>.Failure(albumMetadataResult.Messages);
        }

        var result = await databaseContext.WithTransaction<ValueResult<Album>>(async databaseContextTransaction =>
        {
            var fileInfo = albumMetadataResult.Value.AlbumArtUrl == null ? null : await fileService.CreateFileFromUrlAsync(albumMetadataResult.Value.AlbumArtUrl);

            if (fileInfo != null && !fileInfo.WasSuccess)
            {
                await databaseContextTransaction.RollbackAsync();
                return ResultMessage.Error("Failed to download album art.");
            }

            var artistsResult =
                await artistCreationService.GetOrCreateArtistsAsync(
                    albumMetadataResult.Value!.ArtistIds
                        .Select(x => new ArtistPost { Source = albumPost.SearchResult.Source, SourceId = x.SourceId, SourceUrlId = x.SourceUrlId })
                        .ToArray());

            if (!artistsResult.WasSuccess)
            {
                await databaseContextTransaction.RollbackAsync();
                return ResultMessage.Error("Failed to create artist data.");
            }

            var albumData = new AlbumData
            {
                AlbumArtId = fileInfo?.Value!.Id,
                AlbumArt = fileInfo == null ? null : await databaseContext.FileInfos.FindAsync(fileInfo.Value!.Id),
                AlternateTitle = albumPost.SearchResult.AlternateAlbumTitle,
                Artists = artistsResult.Value!.Where(x => x != null).Select(x => new AlbumArtistData { ArtistId = x!.Id }).ToArray(),
                ReleaseDate = albumMetadataResult.Value.ReleaseDate,
                ReleaseYear = albumMetadataResult.Value.ReleaseYear,
                Source = albumPost.SearchResult.Source,
                SourceId = albumPost.SearchResult.AlbumId,
                SourceUrlId = albumPost.SearchResult.AlbumUrlId,
                Title = albumPost.SearchResult.AlbumTitle,
                TrackCount = (ushort)albumMetadataResult.Value.Tracks.Count,
                Tracks = ConvertToTrackDatas(albumPost.SearchResult.Source, albumMetadataResult.Value.Tracks, albumPost.TracksToRequest),
                FolderName = await GenerateUniqueFolderNameAsync(albumPost.SearchResult.AlbumTitle, albumMetadataResult.Value.ReleaseYear)
            };

            databaseContext.Albums.Add(albumData);
            var hasFileId = await databaseContext.FileInfos.AnyAsync(x => x.Id == 1);
            await databaseContext.SaveChangesAsync();

            var mappedAlbum = metadataMapper.Map(albumData);
            mappedAlbum.Artists = artistsResult.Value!;

            return mappedAlbum;
        });

        if (result.WasSuccess)
        {
            foreach (var track in result.Value.Tracks)
            {
                await trackMetadataTaskQueuingService.QueueAsync(track.Id);
            }
        }

        return result;
    }

    private async Task<string> GenerateUniqueFolderNameAsync(string albumName, int? albumYear)
    {
        var escapedAlbumName = EscapePathItem(albumName);
        var resultWithoutCounter = albumYear == null ? escapedAlbumName : $"{escapedAlbumName} ({albumYear.Value})";
        var result = resultWithoutCounter;
        var counter = 1;

        while (await databaseContext.Albums.AnyAsync(x => x.FolderName == result))
        {
            counter++;
            result = resultWithoutCounter + " " + counter;
        }

        return result;
    }

    private static IList<TrackData> ConvertToTrackDatas(string providerName, IList<AlbumTrackMetadata> albumTrackMetadata, TracksToRequest tracksToRequest)
    {
        return albumTrackMetadata.Select(x => new TrackData
        {
            MetadataSource = providerName,
            MetadataSourceId = x.Ids.SourceId,
            MetadataSourceUrlId = x.Ids.SourceUrlId,
            Title = x.Title,
            IsRequested = tracksToRequest == TracksToRequest.All,
            Status = TrackStatus.MissingMetadata
        }).ToArray();
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
