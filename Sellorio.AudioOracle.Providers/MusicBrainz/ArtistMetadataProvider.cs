using Microsoft.Extensions.Caching.Memory;
using Sellorio.AudioOracle.Library.Comparers;
using Sellorio.AudioOracle.Models.Metadata;
using Sellorio.AudioOracle.Providers.Models;
using Sellorio.AudioOracle.Providers.MusicBrainz.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Providers.MusicBrainz;

internal class ArtistMetadataProvider(HttpClient httpClient) : IArtistMetadataProvider
{
    private static readonly MemoryCache _cache = new(new MemoryCacheOptions());
    private static readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

    public async Task<IList<ArtistMetadata>> GetArtistMetadataAsync(string[] ids)
    {
        var result = new List<ArtistMetadata>(ids.Length);

        foreach (var id in ids)
        {
            var guid = Guid.Parse(id);

            var artist =
                await ProviderHelper.GetWithCacheAndRateLimitAsync(
                    _cache,
                    _cacheDuration,
                    RateLimiters.MusicBrainz,
                    guid,
                    GetArtistAsync);

            if (artist == null)
            {
                result.Add(null);
                continue;
            }

            var resultItem = new ArtistMetadata
            {
                Name = artist.Name,
                Type = Enum.Parse<ArtistType>(artist.Type),
                Country = artist.Area?.Name,
                CountryCode = artist.Country,
                Gender =
                    string.IsNullOrEmpty(artist.Gender)
                        ? ArtistGender.Unspecified
                        : (Enum.TryParse<ArtistGender>(artist.Gender, out var gender) ? gender : ArtistGender.Other),
                Aliases = GetAliases(artist)
            };

            result.Add(resultItem);
        }

        return result;
    }

    private async Task<ArtistDto> GetArtistAsync(Guid id)
    {
        var responseMessage = await httpClient.GetAsync($"artist/{id}?fmt=json&inc=aliases");

        if (responseMessage.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        responseMessage.EnsureSuccessStatusCode();

        var json = await responseMessage.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ArtistDto>(json, Constants.JsonOptions);
    }

    private static string[] GetAliases(ArtistDto artist)
    {
        var result =
            Enumerable.Concat([artist.Name, artist.SortName], artist.Aliases.Select(x => x.Name))
                .Distinct(NameComparer.Instance)
                .Except([artist.Name])
                .ToArray();

        return result;
    }
}
