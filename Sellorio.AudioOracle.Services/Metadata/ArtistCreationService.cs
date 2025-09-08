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
using Sellorio.AudioOracle.ServiceInterfaces.Metadata;

namespace Sellorio.AudioOracle.Services.Metadata;

internal class ArtistCreationService(DatabaseContext databaseContext, IMetadataMapper metadataMapper, IProviderInvocationService providerInvocationService) : IArtistCreationService
{
    public async Task<ValueResult<IList<Artist?>>> GetOrCreateArtistsAsync(IList<ArtistPost> artistPosts)
    {
        var result = new List<Artist?>(artistPosts.Count);

        var sourceIds = artistPosts.Select(x => x.SourceId).ToArray();
        var providerNames = artistPosts.Select(x => x.Source).ToArray();

        var existingArtists =
            await databaseContext.Artists
                .AsNoTracking()
                .Where(x => providerNames.Contains(x.Source) && sourceIds.Contains(x.SourceId))
                .ToDictionaryAsync(x => (x.Source, x.SourceId), x => x);

        var missingArtistPosts = new List<ArtistPost>();

        for (var i = 0; i < artistPosts.Count; i++)
        {
            var artistPost = artistPosts[i];

            if (!existingArtists.TryGetValue((artistPost.Source, artistPost.SourceId), out var existingArtist))
            {
                missingArtistPosts.Add(artistPost);
            }
        }

        var missingArtistPostsGroupedBySource = missingArtistPosts.GroupBy(x => x.Source).ToList();

        var newMetadata = new Dictionary<(string Source, string SourceId), ArtistMetadata?>();

        foreach (var group in missingArtistPostsGroupedBySource)
        {
            var groupIds = group.Select(x => x.SourceId).ToArray();

            var missingArtistsMetadataResult =
                await providerInvocationService.InvokeAsync<IArtistMetadataProvider, IList<ArtistMetadata?>>(
                    group.Key,
                    x => x.GetArtistMetadataAsync(groupIds));

            if (missingArtistsMetadataResult.WasSuccess ||
                missingArtistsMetadataResult.Value!.Any(x => x == null))
            {
                return ResultMessage.Error("Failed to retrieve artist metadata from provider.");
            }

            for (var i = 0; i < groupIds.Length; i++)
            {
                var metadata = missingArtistsMetadataResult.Value[i];
                newMetadata[(group.Key, groupIds[i])] = metadata;
            }
        }

        foreach (var artistPost in artistPosts)
        {
            var key = (artistPost.Source, artistPost.SourceId);

            if (existingArtists.TryGetValue(key, out var existingArtist))
            {
                result.Add(metadataMapper.Map(existingArtist));
                continue;
            }

            var metadata = newMetadata[key];

            if (metadata == null)
            {
                result.Add(null);
                continue;
            }

            existingArtist = new()
            {
                ArtistNames =
                    Enumerable
                        .Concat([metadata!.Name], metadata.Aliases)
                        .Select(x => new ArtistNameData { Name = x })
                        .ToArray(),
                Country = metadata.Country,
                CountryCode = metadata.CountryCode,
                Gender = metadata.Gender,
                Name = metadata.Name,
                Source = artistPost.Source,
                SourceId = artistPost.SourceId,
                SourceUrlId = artistPost.SourceUrlId,
                Type = metadata.Type
            };

            databaseContext.Artists.Add(existingArtist);
            await databaseContext.SaveChangesAsync();

            result.Add(metadataMapper.Map(existingArtist));
        }

        return result;
    }
}
