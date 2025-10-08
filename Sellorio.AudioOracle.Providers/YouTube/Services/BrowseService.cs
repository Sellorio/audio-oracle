using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Providers.YouTube.Services;

internal class BrowseService(IApiService apiService) : IBrowseService
{
    private static readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);
    private static readonly MemoryCache _browseToAlbumIdCache = new(new MemoryCacheOptions());

    public async Task<string> ResolveAlbumIdFromBrowseIdAsync(string browseId)
    {
        return await ProviderHelper.GetWithCacheAsync(_browseToAlbumIdCache, _cacheDuration, browseId, InnerResolveAlbumIdFromBrowseIdAsync);
    }

    public async Task<string> ResolveChannelIdFromBrowseIdAsync(string browseId)
    {
        var response = await apiService.PostWithContextAsync("/browse?prettyPrint=false", new { browseId });
        var header = response["header"] ?? throw new InvalidOperationException("Unable to parse browse page.");
        var channelId = header["musicImmersiveHeaderRenderer"]!["subscriptionButton"]!["subscribeButtonRenderer"]!.Get<string>("channelId")!;
        return channelId;
    }

    private async Task<string> InnerResolveAlbumIdFromBrowseIdAsync(string browseId)
    {
        var response = await apiService.PostWithContextAsync("/browse?prettyPrint=false", new { browseId });
        var contents = response["contents"] ?? throw new InvalidOperationException("Unable to parse browse page.");
        var items = contents["twoColumnBrowseResultsRenderer"]!["secondaryContents"]!["sectionListRenderer"]!["contents"]![0]!["musicShelfRenderer"]!["contents"]!;
        var itemWithNavigationEndpoint = items.Select(x => x!["musicResponsiveListItemRenderer"]!["overlay"]!["musicItemThumbnailOverlayRenderer"]!["content"]!["musicPlayButtonRenderer"]!["playNavigationEndpoint"]).First(x => x != null);
        var playlistId = itemWithNavigationEndpoint!["watchEndpoint"]!.Get<string>("playlistId")!;
        return playlistId;
    }
}
