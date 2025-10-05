using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Sellorio.AudioOracle.Providers.SoundCloud.Services;

internal partial class SoundCloudClientIdService(HttpClient httpClient) : ISoundCloudClientIdService
{
    private static readonly MemoryCache _cache = new(new MemoryCacheOptions());
    private static readonly TimeSpan _cacheDuration = TimeSpan.FromHours(1);
    private static readonly object _key = new();

    public async Task<string> GetClientIdAsync()
    {
        return await ProviderHelper.GetWithCacheAsync(_cache, _cacheDuration, _key, x => GetClientIdWithoutCacheAsync());
    }

    private async Task<string> GetClientIdWithoutCacheAsync()
    {
        var html = await httpClient.GetStringAsync("https://soundcloud.com/");
        var javaScriptFileNames = JavaScriptFileNameRegex().Matches(html).Select(x => x.Groups[1].Value).ToArray();

        foreach (var javaScriptFileName in javaScriptFileNames)
        {
            var javaScript = await httpClient.GetStringAsync($"https://a-v2.sndcdn.com/assets/{javaScriptFileName}");

            var clientIdMatch = ClientIdRegex().Match(javaScript);

            if (clientIdMatch.Success)
            {
                return clientIdMatch.Groups[1].Value;
            }
        }

        throw new InvalidOperationException("Failed to find client_id.");
    }

    [GeneratedRegex(@"<script crossorigin src=""https:\/\/a-v2\.sndcdn\.com\/assets\/([a-zA-Z0-9-]+\.js)""><\/script>")]
    private static partial Regex JavaScriptFileNameRegex();

    [GeneratedRegex(@"""client_id=([a-zA-Z0-9]+)""")]
    private static partial Regex ClientIdRegex();
}
