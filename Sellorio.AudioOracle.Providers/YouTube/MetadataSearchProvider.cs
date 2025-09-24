using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.ApiTools;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Search;
using Sellorio.AudioOracle.Providers.Models;
using Sellorio.AudioOracle.Providers.YouTube.Services;

namespace Sellorio.AudioOracle.Providers.YouTube;

internal class MetadataSearchProvider(IApiService apiService, IBrowseService browseService) : IMetadataSearchProvider
{
    public string ProviderName => Constants.ProviderName;

    public async Task<ValueResult<PagedList<MetadataSearchResult>>> SearchForMetadataAsync(string searchText, int pageSize)
    {
        var albumSearchResults = await SearchAsync(searchText, Constants.SearchByAlbumsParams, ConvertAlbumResultAsync);
        var songSearchResults = await SearchAsync(searchText, Constants.SearchBySongsParams, ConvertSongResultAsync);

        var albumsToTake =
            Math.Min(
                albumSearchResults.Length,
                (pageSize / 2) + ((pageSize / 2) - Math.Min(pageSize / 2, songSearchResults.Length)));

        var songsToTake = pageSize - albumsToTake;

        return new PagedList<MetadataSearchResult>
        {
            Items =
                Enumerable.Concat(
                    albumSearchResults.Take(albumsToTake),
                    songSearchResults.Take(songsToTake))
                        .ToArray(),
            Page = 1,
            PageSize = pageSize
        };
    }

    private async Task<MetadataSearchResult[]> SearchAsync(string searchText, string @params, Func<JsonNavigator, Task<MetadataSearchResult>> searchResultConverter)
    {
        var searchResponse = await apiService.PostWithContextAsync("/search?prettyPrint=false", new { @params, query = searchText });
        var searchResultsJson =
            searchResponse["contents"]?["tabbedSearchResultsRenderer"]?["tabs"]?[0]?["tabRenderer"]?["content"]?["sectionListRenderer"]?["contents"]?[0]?["musicShelfRenderer"]?["contents"]
                ?? throw new InvalidOperationException("Unable to parse search results.");

        var results = new MetadataSearchResult[searchResultsJson.ArrayLength];

        for (var i = 0; i < searchResultsJson.ArrayLength; i++)
        {
            results[i] =
                await searchResultConverter.Invoke(
                    searchResultsJson[i]
                        ?? throw new InvalidOperationException("Unable to parse search results."));
        }

        return results;
    }

    private Task<MetadataSearchResult> ConvertAlbumResultAsync(JsonNavigator navigator)
    {
        var root =
            navigator["musicResponsiveListItemRenderer"]
                ?? throw new InvalidOperationException("Unable to parse search results.");

        var thumbnailUrl =
            root["thumbnail"]?["musicThumbnailRenderer"]?["thumbnail"]?["thumbnails"]?.NthFromLast(0)?.Get<string>("url")
                ?? throw new InvalidOperationException("Unable to parse search results.");

        var infoRoot =
            root["flexColumns"]
                ?? throw new InvalidOperationException("Unable to parse search results.");

        var titleSection =
            infoRoot[0]?["musicResponsiveListItemFlexColumnRenderer"]?["text"]?["runs"]?[0]
                ?? throw new InvalidOperationException("Unable to parse search results.");

        var title =
            titleSection.Get<string>("text")
                ?? throw new InvalidOperationException("Unable to parse search results.");

        var artistsSection =
            infoRoot[1]?["musicResponsiveListItemFlexColumnRenderer"]?["text"]?["runs"]
                ?? throw new InvalidOperationException("Unable to parse search results.");

        var artists = new List<string>();

        for (var i = 2; i < artistsSection.ArrayLength - 2; i++)
        {
            var artistSection =
                artistsSection[i]
                    ?? throw new InvalidOperationException("Unable to parse search results.");

            var artistName =
                artistSection.Get<string>("text")
                    ?? throw new InvalidOperationException("Unable to parse search results.");

            artists.Add(artistName);
        }

        var albumId =
            root["menu"]?["menuRenderer"]?["items"]?[0]?["menuNavigationItemRenderer"]?["navigationEndpoint"]?["watchPlaylistEndpoint"]?.Get<string>("playlistId")
                ?? throw new InvalidOperationException("Unable to parse search results.");

        var albumBrowseId = root["navigationEndpoint"]!["browseEndpoint"]!.Get<string>("browseId")!;

        return Task.FromResult(new MetadataSearchResult
        {
            AlbumArtUrl = thumbnailUrl,
            AlbumIds = new() { SourceId = albumId, SourceUrlId = albumBrowseId },
            AlbumTitle = title,
            AlternateAlbumTitle = null,
            Title = title,
            AlternateTitle = null,
            ArtistNames = artists,
            Source = Constants.ProviderName,
            Type = SearchResultType.Album
        });
    }

    private async Task<MetadataSearchResult> ConvertSongResultAsync(JsonNavigator navigator)
    {
        var root =
            navigator["musicResponsiveListItemRenderer"]
                ?? throw new InvalidOperationException("Unable to parse search results.");

        var thumbnailUrl =
            root["thumbnail"]?["musicThumbnailRenderer"]?["thumbnail"]?["thumbnails"]?.NthFromLast(0)?.Get<string>("url")
                ?? throw new InvalidOperationException("Unable to parse search results.");

        var infoRoot =
            root["flexColumns"]
                ?? throw new InvalidOperationException("Unable to parse search results.");

        var titleSection =
            infoRoot[0]?["musicResponsiveListItemFlexColumnRenderer"]?["text"]?["runs"]?[0]
                ?? throw new InvalidOperationException("Unable to parse search results.");

        var title =
            titleSection.Get<string>("text")
                ?? throw new InvalidOperationException("Unable to parse search results.");

        var albumArtistSection =
            infoRoot[1]?["musicResponsiveListItemFlexColumnRenderer"]?["text"]?["runs"]
                ?? throw new InvalidOperationException("Unable to parse search results.");

        var artists = new List<string>();
        (string Id, string BrowseId, string Title)? album = null;

        for (var i = 0; i < albumArtistSection.ArrayLength; i += 2)
        {
            if (i == albumArtistSection.ArrayLength - 1) // track duration/album year
            {
                // we don't care about track duration in search
            }
            else if (i == albumArtistSection.ArrayLength - 3) // album
            {
                var albumSection =
                    albumArtistSection[i]
                        ?? throw new InvalidOperationException("Unable to parse search results.");

                var albumTitle =
                    albumSection.Get<string>("text")
                        ?? throw new InvalidOperationException("Unable to parse search results.");

                var albumBrowseId =
                    albumSection["navigationEndpoint"]?["browseEndpoint"]?.Get<string>("browseId")
                        ?? throw new InvalidOperationException("Unable to parse search results.");

                var albumId = await browseService.ResolveAlbumIdFromBrowseIdAsync(albumBrowseId);

                album = (albumId, albumBrowseId, albumTitle);
            }
            else // artist
            {
                var artistSection =
                    albumArtistSection[i]
                        ?? throw new InvalidOperationException("Unable to parse search results.");

                var artistName =
                    artistSection.Get<string>("text")
                        ?? throw new InvalidOperationException("Unable to parse search results.");

                artists.Add(artistName);
            }
        }

        return new MetadataSearchResult
        {
            AlbumArtUrl = thumbnailUrl,
            AlbumIds = new() { SourceId = album!.Value.Id, SourceUrlId = album.Value.BrowseId },
            AlbumTitle = album.Value.Title,
            AlternateAlbumTitle = null,
            Title = title,
            // to avoid delays processing a search, we'll ignore alternate title in this step - it'll be handled when getting track metadata during an add
            AlternateTitle = null,
            ArtistNames = artists,
            Source = Constants.ProviderName,
            Type = SearchResultType.Track
        };
    }
}
