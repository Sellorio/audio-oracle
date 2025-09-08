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
using Sellorio.AudioOracle.Models.Search;
using Sellorio.AudioOracle.Providers;
using Sellorio.AudioOracle.Providers.Models;
using Sellorio.AudioOracle.ServiceInterfaces.Metadata;
using Sellorio.AudioOracle.Services.Content;

namespace Sellorio.AudioOracle.Services.Metadata;

internal class AlbumCreationService(
    DatabaseContext databaseContext,
    IMetadataMapper metadataMapper,
    IProviderInvocationService providerInvocationService,
    IFileService fileService,
    IArtistCreationService artistCreationService) : IAlbumCreationService
{
    public async Task<ValueResult<Album>> CreateAlbumFromSearchResultAsync(SearchResult searchResult)
    {
        var existingAlbum =
            await databaseContext.Albums
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Source == searchResult.Source && x.SourceId == searchResult.AlbumId);

        if (existingAlbum != null)
        {
            return metadataMapper.Map(existingAlbum);
        }

        var albumMetadataResult =
            await providerInvocationService.InvokeAsync<IAlbumMetadataProvider, AlbumMetadata>(
                searchResult.Source,
                x => x.GetAlbumMetadataAsync(new() { SourceId = searchResult.AlbumId, SourceUrlId = searchResult.AlbumUrlId }));

        if (!albumMetadataResult.WasSuccess)
        {
            return ValueResult<Album>.Failure(albumMetadataResult.Messages);
        }

        return await databaseContext.WithTransaction<ValueResult<Album>>(async databaseContextTransaction =>
        {
            var fileInfo = searchResult.AlbumArtUrl == null ? null : await fileService.CreateFileFromUrlAsync(searchResult.AlbumArtUrl);

            if (fileInfo != null && !fileInfo.WasSuccess)
            {
                await databaseContextTransaction.RollbackAsync();
                return ResultMessage.Error("Failed to download album art.");
            }

            var artistsResult =
                await artistCreationService.GetOrCreateArtistsAsync(
                    albumMetadataResult.Value!.ArtistIds
                        .Select(x => new ArtistPost { Source = searchResult.Source, SourceId = x.SourceId, SourceUrlId = x.SourceUrlId })
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
                AlternateTitle = searchResult.AlternateAlbumTitle,
                Artists = artistsResult.Value!.Where(x => x != null).Select(x => new AlbumArtistData { ArtistId = x!.Id }).ToArray(),
                ReleaseDate = albumMetadataResult.Value.ReleaseDate,
                ReleaseYear = albumMetadataResult.Value.ReleaseYear,
                Source = searchResult.Source,
                SourceId = searchResult.AlbumId,
                SourceUrlId = searchResult.AlbumUrlId,
                Title = searchResult.AlbumTitle,
                TrackCount = (ushort)albumMetadataResult.Value.Tracks.Count,
                Tracks = ConvertToTrackDatas(searchResult.Source, albumMetadataResult.Value.Tracks),
                FolderName = await GenerateUniqueFolderNameAsync(searchResult.AlbumTitle, albumMetadataResult.Value.ReleaseYear)
            };

            databaseContext.Albums.Add(albumData);
            await databaseContext.SaveChangesAsync();

            var mappedAlbum = metadataMapper.Map(albumData); // TODO: Check if artists are fully populated

            return mappedAlbum;
        });
    }

    private async Task<string> GenerateUniqueFolderNameAsync(string albumName, int? albumYear)
    {
        var escapedAlbumName = EscapePathItem(albumName);
        var resultWithoutCounter = albumYear == null ? escapedAlbumName : $"{escapedAlbumName} ({albumYear.Value})";
        var result = resultWithoutCounter;
        var counter = 1;

        while (await databaseContext.Albums.AnyAsync(x => x.FolderName.Equals(result, StringComparison.OrdinalIgnoreCase)))
        {
            counter++;
            result = resultWithoutCounter + " " + counter;
        }

        return result;
    }

    private static IList<TrackData> ConvertToTrackDatas(string providerName, IList<AlbumTrackMetadata> albumTrackMetadata)
    {
        return albumTrackMetadata.Select(x => new TrackData
        {
            MetadataSource = providerName,
            MetadataSourceId = x.Ids.SourceId,
            MetadataSourceUrlId = x.Ids.SourceUrlId,
            Title = x.Title,
            IsRequested = true,
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
