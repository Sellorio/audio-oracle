using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Sellorio.AudioOracle.Library;
using Sellorio.AudioOracle.Library.ApiTools;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Search;
using Sellorio.AudioOracle.Providers.Models;
using Sellorio.AudioOracle.Providers.YouTube.Services;

namespace Sellorio.AudioOracle.Providers.YouTube;

internal class MetadataSearchProvider(IPageService pageService, IBrowseService browseService) : IMetadataSearchProvider
{
    public string ProviderName => Constants.ProviderName;

    public async Task<ValueResult<PagedList<MetadataSearchResult>>> SearchForMetadataAsync(string searchText, int pageSize)
    {
        var pageDatas = await pageService.GetPageInitialDataAsync("https://music.youtube.com/search?q=" + WebUtility.UrlEncode(searchText));
        var pageData = pageDatas[1];

        var resultsContainer = pageData["contents"]!["tabbedSearchResultsRenderer"]!["tabs"]![0]!["tabRenderer"]!["content"]!["sectionListRenderer"]!["contents"]!;
        var mainResult = resultsContainer[0]!["musicCardShelfRenderer"]!;
        var otherResults = resultsContainer[1]!["musicShelfRenderer"]!;

        var resultItems = new List<MetadataSearchResult>(20);

        var mainResultItem = await ConvertResultAsync(mainResult, isCard: true);

        if (mainResultItem != null)
        {
            resultItems.Add(mainResultItem);
        }

        foreach (var otherResult in otherResults["contents"]!.AsEnumerable())
        {
            var otherResultItem = await ConvertResultAsync(otherResult!["musicResponsiveListItemRenderer"]!, isCard: false);

            if (otherResultItem != null)
            {
                resultItems.Add(otherResultItem);
            }
        }

        return new PagedList<MetadataSearchResult>
        {
            Items = resultItems,
            Page = 1,
            PageSize = Math.Max(pageSize, resultItems.Count)
        };
    }

    private async Task<MetadataSearchResult?> ConvertResultAsync(JsonNavigator navigator, bool isCard)
    {
        var thumbnailUrl =
            navigator["thumbnail"]?["musicThumbnailRenderer"]?["thumbnail"]?["thumbnails"]?.NthFromLast(0)?.Get<string>("url")
                ?? throw new InvalidOperationException("Unable to parse search results.");

        var resultType =
            isCard
                ? navigator["subtitle"]!["runs"]![0]!.Get<string>("text")!
                : navigator["flexColumns"]![1]!["musicResponsiveListItemFlexColumnRenderer"]!["text"]!["runs"]![0]!.Get<string>("text")!;

        if (resultType is not "Album" and not "Song")
        {
            return null;
        }

        var title =
            isCard
                ? navigator["title"]!["runs"]![0]!.Get<string>("text")!
                : navigator["flexColumns"]![0]!["musicResponsiveListItemFlexColumnRenderer"]!["text"]!["runs"]![0]!.Get<string>("text")!;

        IList<string> artistNames;

        if (isCard)
        {
            var subtitleRuns = navigator["subtitle"]!["runs"]!;
            var artistElements = subtitleRuns.AsEnumerable().Skip(2).Take(subtitleRuns.ArrayLength - 4).SkipEvery(2);
            artistNames = artistElements.Select(x => x!.Get<string>("text")!).ToArray();
        }
        else
        {
            var subtitleRuns = navigator["flexColumns"]![1]!["musicResponsiveListItemFlexColumnRenderer"]!["text"]!["runs"]!;
            var artistElements = subtitleRuns.AsEnumerable().Skip(2).Take(subtitleRuns.ArrayLength - 4).SkipEvery(2);
            artistNames = artistElements.Select(x => x!.Get<string>("text")!).ToArray();
        }

        string albumId;
        string albumBrowseId;
        string albumTitle;

        if (resultType == "Album")
        {
            albumId =
                isCard
                    ? navigator["buttons"]![0]!["buttonRenderer"]!["command"]!["watchEndpoint"]!.Get<string>("playlistId")!
                    : navigator["overlay"]!["musicItemThumbnailOverlayRenderer"]!["content"]!["musicPlayButtonRenderer"]!["playNavigationEndpoint"]!["watchEndpoint"]!.Get<string>("playlistId")!;

            albumBrowseId =
                isCard
                    ? navigator["title"]!["runs"]![0]!["navigationEndpoint"]!["browseEndpoint"]!.Get<string>("browseId")!
                    : navigator["navigationEndpoint"]!["browseEndpoint"]!.Get<string>("browseId")!;

            albumTitle = title;
        }
        else
        {
            albumBrowseId =
                navigator["menu"]!["menuRenderer"]!["items"]!.Where(x => x["menuNavigationItemRenderer"]?["icon"]!.Get<string>("iconType") == "ALBUM").First()["menuNavigationItemRenderer"]!["navigationEndpoint"]!["browseEndpoint"]!.Get<string>("browseId")!;

            var albumBasicInfo = await browseService.ResolveAlbumBasicInfoFromBrowseIdAsync(albumBrowseId);

            albumId = albumBasicInfo.AlbumId;
            albumTitle = albumBasicInfo.Title;
        }

        return new MetadataSearchResult
        {
            Title = title,
            AlbumArtUrl = thumbnailUrl,
            AlbumIds = new() { SourceId = albumId, SourceUrlId = albumBrowseId },
            AlbumTitle = albumTitle,
            AlternateAlbumTitle = null,
            AlternateTitle = null,
            ArtistNames = artistNames,
            Source = ProviderName,
            Type = resultType == "Album" ? SearchResultType.Album : SearchResultType.Track
        };
    }
}
