using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.ApiTools;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Metadata;
using Sellorio.AudioOracle.Providers.Models;
using Sellorio.AudioOracle.Providers.YouTube.Services;

namespace Sellorio.AudioOracle.Providers.YouTube;

internal class DownloadSearchProvider(IApiService apiService) : IDownloadSearchProvider
{
    public string ProviderName => Constants.ProviderName;

    public async Task<ValueResult<PagedList<DownloadSearchResult>>> SearchForDownloadAsync(DownloadSearchCriteria searchCriteria, int pageSize)
    {
        var searchResults =
            await SearchAsync(
                searchCriteria.TrackTitle + " " + searchCriteria.MainArtist,
                Constants.SearchBySongsParams,
                ConvertSongResult);

        return new PagedList<DownloadSearchResult>
        {
            Items = searchResults.Take(pageSize).ToArray(),
            Page = 1,
            PageSize = pageSize
        };
    }

    private async Task<DownloadSearchResult[]> SearchAsync(string searchText, string @params, Func<JsonNavigator, DownloadSearchResult> searchResultConverter)
    {
        var searchResponse = await apiService.PostWithContextAsync("/search?prettyPrint=false", new { @params, query = searchText });
        var searchResultsJson =
            searchResponse["contents"]?["tabbedSearchResultsRenderer"]?["tabs"]?[0]?["tabRenderer"]?["content"]?["sectionListRenderer"]?["contents"]?[0]?["musicShelfRenderer"]?["contents"]
                ?? throw new InvalidOperationException("Unable to parse search results.");

        var results = new DownloadSearchResult[searchResultsJson.ArrayLength];

        for (var i = 0; i < searchResultsJson.ArrayLength; i++)
        {
            results[i] =
                searchResultConverter.Invoke(
                    searchResultsJson[i]
                        ?? throw new InvalidOperationException("Unable to parse search results."));
        }

        return results;
    }

    private static DownloadSearchResult ConvertSongResult(JsonNavigator navigator)
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

        var videoId = titleSection["navigtionEndpoint"]!["watchEndpoint"]!.Get<string>("videoId")!;

        var albumArtistSection =
            infoRoot[1]?["musicResponsiveListItemFlexColumnRenderer"]?["text"]?["runs"]
                ?? throw new InvalidOperationException("Unable to parse search results.");

        var artists = new List<string>();
        string? album = null;

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

                album = albumTitle;
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

        return new DownloadSearchResult
        {
            Source = Constants.ProviderName,
            Ids = new() { SourceId = videoId, SourceUrlId = videoId },
            AlbumArtUrl = thumbnailUrl,
            AlbumTitle = album ?? throw new InvalidOperationException("Unable to parse search results."),
            ArtistNames = artists,
            Title = title
        };
    }
}
