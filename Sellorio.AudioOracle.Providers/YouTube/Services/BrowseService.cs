using Microsoft.Extensions.Caching.Memory;
using Sellorio.AudioOracle.Providers.YouTube.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Providers.YouTube.Services;

internal class BrowseService(IApiService apiService) : IBrowseService
{
    private static readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);
    private static readonly MemoryCache _browseToAlbumIdCache = new(new MemoryCacheOptions());

    public async Task<AlbumBrowseBasicInfo> ResolveAlbumBasicInfoFromBrowseIdAsync(string browseId)
    {
        return await ProviderHelper.GetWithCacheAsync(_browseToAlbumIdCache, _cacheDuration, browseId, InnerResolveAlbumIdFromBrowseIdAsync);
    }

    public async Task<string> ResolveChannelIdFromBrowseIdAsync(string browseId)
    {
        var response = await apiService.PostWithContextAsync("/browse?prettyPrint=false", new { browseId });
        var header = response["header"] ?? throw new InvalidOperationException("Unable to parse browse page.");
        var headerRenderer = header["musicImmersiveHeaderRenderer"] ?? header["musicVisualHeaderRenderer"]!;
        var channelId = headerRenderer["subscriptionButton"]!["subscribeButtonRenderer"]!.Get<string>("channelId")!;
        return channelId;
    }

    private async Task<AlbumBrowseBasicInfo> InnerResolveAlbumIdFromBrowseIdAsync(string browseId)
    {
        var response = await apiService.PostWithContextAsync("/browse?prettyPrint=false", new { browseId });
        var contents = response["contents"] ?? throw new InvalidOperationException("Unable to parse browse page.");
        var headerElement = contents["twoColumnBrowseResultsRenderer"]!["tabs"]![0]!["tabRenderer"]!["content"]!["sectionListRenderer"]!["contents"]![0]!["musicResponsiveHeaderRenderer"]!;
        var playlistId = headerElement["buttons"]!.Select(x => x!["musicPlayButtonRenderer"]).First(x => x != null)!["playNavigationEndpoint"]!["watchPlaylistEndpoint"]!.Get<string>("playlistId")!;
        var playlistName = headerElement["title"]!["runs"]![0]!.Get<string>("text")!;

        return new AlbumBrowseBasicInfo
        {
            AlbumId = playlistId,
            Title = playlistName
        };
    }
}
