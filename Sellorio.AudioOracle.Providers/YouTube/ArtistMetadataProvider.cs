using System.Collections.Generic;
using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Metadata;
using Sellorio.AudioOracle.Providers.Models;
using Sellorio.AudioOracle.Providers.YouTube.Services;

namespace Sellorio.AudioOracle.Providers.YouTube;

internal class ArtistMetadataProvider(IApiService apiService) : IArtistMetadataProvider
{
    public string ProviderName => Constants.ProviderName;

    public async Task<ValueResult<IList<ArtistMetadata?>>> GetArtistMetadataAsync(string[] ids)
    {
        var result = new ArtistMetadata[ids.Length];

        for (var i = 0; i < ids.Length; i++)
        {
            var artistId = ids[i];

            if (artistId.StartsWith(Constants.UnregisteredArtistIdPrefix))
            {
                // for text-only artists, the name will be included in the id when retrieving the artist ids for a track/album
                var artistNameStart = artistId.LastIndexOf(':') + 1;

                result[i] = new ArtistMetadata
                {
                    Aliases = [],
                    Country = null,
                    CountryCode = null,
                    Gender = null,
                    Name = artistId[artistNameStart..],
                    Type = ArtistType.Unknown
                };
            }
            else
            {
                // channelId is a valid browseId as well! There is no dedicated endpoint for getting artist information.
                var artistData = await apiService.PostWithContextAsync("/browse?prettyPrint=false", new { browseId = artistId });
                var header = artistData["header"]!["musicImmersiveHeaderRenderer"]!;
                var artistName = header["title"]!["runs"]![0]!.Get<string>("text")!;

                result[i] = new ArtistMetadata
                {
                    Aliases = [],
                    Country = null,
                    CountryCode = null,
                    Gender = null,
                    Name = artistName,
                    Type = ArtistType.Unknown
                };
            }
        }

        return result;
    }
}
