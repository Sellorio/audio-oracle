using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Providers.YouTube.Services;

internal class BrowseService(IPageService pageService) : IBrowseService
{
    private static readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);
    private static readonly MemoryCache _browseToAlbumIdCache;

    public async Task<string> ResolveAlbumIdFromBrowseIdAsync(string browseId)
    {
        return await ProviderHelper.GetWithCacheAsync(_browseToAlbumIdCache, _cacheDuration, browseId, InnerResolveAlbumIdFromBrowseIdAsync);
    }

    private async Task<string> InnerResolveAlbumIdFromBrowseIdAsync(string browseId)
    {
        var browsePageData = await pageService.GetPageInitialDataAsync($"https://music.youtube.com/browse/{browseId}");
        var infoPage = browsePageData[1];
        var contents = infoPage["contents"] ?? throw new InvalidOperationException("Unable to get album id. You may need to refresh your cookies.txt.");

        var playlistParent =
            contents["twoColumnBrowseResultsRenderer"]["secondaryContents"]["musicResponsiveListItemRenderer"]?["overlay"]["musicItemThumbnailOverlayRenderer"]["content"]["musicPlayButtonRenderer"]["playNavigationEndpoint"]["watchEndpoint"] ??
            contents["twoColumnBrowseResultsRenderer"]["secondaryContents"]["sectionListRenderer"]["contents"][0]["musicShelfRenderer"]["contents"].Select(x => x["musicResponsiveListItemRenderer"]["overlay"]["musicItemThumbnailOverlayRenderer"]["content"]["musicPlayButtonRenderer"]["playNavigationEndpoint"]).FirstOrDefault(x => x != null)["watchEndpoint"];

        var playlistId = playlistParent.Get<string>("playlistId");
        return playlistId;
    }
}
