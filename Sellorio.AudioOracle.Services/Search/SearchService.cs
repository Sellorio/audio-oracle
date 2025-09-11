using Microsoft.EntityFrameworkCore;
using Sellorio.AudioOracle.Data;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Search;
using Sellorio.AudioOracle.Providers;
using Sellorio.AudioOracle.Providers.Models;
using Sellorio.AudioOracle.ServiceInterfaces.Search;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Services.Search;

internal class SearchService(DatabaseContext databaseContext, IProviderInvocationService providerInvocationService) : ISearchService
{
    public async Task<ValueResult<IList<SearchResult>>> SearchAsync(string searchText)
    {
        var providersSearchResult =
            await providerInvocationService.InvokeAllAsync<IMetadataSearchProvider, PagedList<MetadataSearchResult>>(
                x => x.SearchForMetadataAsync(searchText, pageSize: 20));

        if (!providersSearchResult.WasSuccess)
        {
            return ValueResult<IList<SearchResult>>.Failure(providersSearchResult.Messages);
        }

        var albumSourceIds = providersSearchResult.Value!.SelectMany(x => x.Items).Select(x => x.AlbumIds.SourceId).Distinct().ToList();
        var existingAlbums = await databaseContext.Albums.Where(x => albumSourceIds.Contains(x.SourceId)).ToDictionaryAsync(x => (x.Source, x.SourceId), x => x);
        var results = new List<SearchResult>(providersSearchResult.Value!.Sum(x => x.Items.Count));

        foreach (var providerResults in providersSearchResult.Value!)
        {
            foreach (var providerResult in providerResults.Items)
            {
                results.Add(new()
                {
                    AlbumArtUrl = providerResult.AlbumArtUrl,
                    AlbumId = providerResult.AlbumIds.SourceId,
                    AlbumTitle = providerResult.AlbumTitle,
                    AlbumUrlId = providerResult.AlbumIds.SourceUrlId,
                    AlternateAlbumTitle = providerResult.AlternateAlbumTitle,
                    AlternateTitle = providerResult.AlternateTitle,
                    Source = providerResult.Source,
                    Status =
                        existingAlbums.ContainsKey((providerResult.Source, providerResult.AlbumIds.SourceId))
                            ? SearchResultStatus.Completed // support for InProgress is not a priority
                            : SearchResultStatus.NotAdded,
                    Title = providerResult.Title,
                    Type = providerResult.Type,
                    ArtistNames = providerResult.ArtistNames
                });
            }
        }

        return ValueResult<IList<SearchResult>>.Success(results, providersSearchResult.Messages);
    }
}
