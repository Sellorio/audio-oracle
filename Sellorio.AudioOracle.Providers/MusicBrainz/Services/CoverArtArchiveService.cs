using Microsoft.Extensions.Caching.Memory;
using Sellorio.AudioOracle.Providers.MusicBrainz.Dtos;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Providers.MusicBrainz.Services;

internal class CoverArtArchiveService(HttpClient httpClient) : ICoverArtArchiveService
{
    private static readonly MemoryCache _cache = new(new MemoryCacheOptions());
    private static readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

    public async Task<string?> GetReleaseArtUrlAsync(Guid musicBrainzReleaseId)
    {
        return
            await ProviderHelper.GetWithCacheAndRateLimitAsync(
                _cache,
                _cacheDuration,
                RateLimiters.CoverArtArchive,
                musicBrainzReleaseId,
                GetReleaseArtUrlWithoutCacheAsync);
    }

    private async Task<string?> GetReleaseArtUrlWithoutCacheAsync(Guid musicBrainzReleaseId)
    {
        var response = await httpClient.GetAsync($"release/{musicBrainzReleaseId}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var dto = await response.Content.ReadFromJsonAsync<ReleaseArtDto>();

        if (dto!.Images == null)
        {
            return null;
        }

        var image = dto!.Images.FirstOrDefault(x => x.Front) ?? dto.Images.FirstOrDefault();

        if (image == null || image.Thumbnails == null)
        {
            return null;
        }

        _ = image.Thumbnails.TryGetValue("500", out var result) ||
            image.Thumbnails.TryGetValue("large", out result);

        return result;
    }
}
