using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sellorio.AudioOracle.Data;
using Sellorio.AudioOracle.Data.Metadata;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Library.Results.Messages;
using Sellorio.AudioOracle.Models.Metadata;
using Sellorio.AudioOracle.Providers;
using Sellorio.AudioOracle.Providers.Models;

namespace Sellorio.AudioOracle.Services.Metadata;

internal class ArtistCreationService(DatabaseContext databaseContext, IMetadataMapper metadataMapper, IProviderInvocationService providerInvocationService) : IArtistCreationService
{
    public async Task<ValueResult<IList<Artist>>> GetOrCreateArtistsAsync(string providerName, IList<ResolvedIds> resolvedIds)
    {
        var result = new List<Artist>(resolvedIds.Count);

        var sourceIds = resolvedIds.Select(x => x.SourceId).ToArray();

        var existingArtists =
            await databaseContext.Artists
                .AsNoTracking()
                .Where(x => x.Source == providerName && sourceIds.Contains(x.SourceId))
                .ToDictionaryAsync(x => x.SourceId, x => x);

        var missingArtistSourceIds = sourceIds.Where(x => existingArtists.All(y => y.Key != x)).ToArray();

        var missingArtistsMetadataResult =
            await providerInvocationService.InvokeAsync<IArtistMetadataProvider, IList<ArtistMetadata?>>(
                providerName,
                x => x.GetArtistMetadataAsync(missingArtistSourceIds));

        if (missingArtistsMetadataResult.WasSuccess ||
            missingArtistsMetadataResult.Value!.Any(x => x == null))
        {
            return ResultMessage.Error("Failed to retrieve artist metadata from provider.");
        }

        foreach (var resolvedId in resolvedIds)
        {
            if (existingArtists.TryGetValue(resolvedId.SourceId, out var artistData))
            {
                result.Add(metadataMapper.Map(artistData));
                continue;
            }

            var indexOfMissingSourceId = missingArtistSourceIds.Index().First(x => x.Item == resolvedId.SourceId).Index;
            var artistMetadata = missingArtistsMetadataResult.Value![indexOfMissingSourceId];

            artistData = new()
            {
                ArtistNames =
                    Enumerable
                        .Concat([artistMetadata!.Name], artistMetadata.Aliases)
                        .Select(x => new ArtistNameData { Name = x })
                        .ToArray(),
                Country = artistMetadata.Country,
                CountryCode = artistMetadata.CountryCode,
                Gender = artistMetadata.Gender,
                Name = artistMetadata.Name,
                Source = providerName,
                SourceId = resolvedId.SourceId,
                SourceUrlId = resolvedId.SourceUrlId,
                Type = artistMetadata.Type
            };

            databaseContext.Artists.Add(artistData);
            await databaseContext.SaveChangesAsync();

            result.Add(metadataMapper.Map(artistData));
        }

        return result;
    }
}
