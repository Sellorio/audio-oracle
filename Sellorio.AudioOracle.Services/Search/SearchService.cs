using Microsoft.EntityFrameworkCore;
using Sellorio.AudioOracle.Data;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Library.Results.Messages;
using Sellorio.AudioOracle.Models.Metadata;
using Sellorio.AudioOracle.Models.Search;
using Sellorio.AudioOracle.Providers;
using Sellorio.AudioOracle.Providers.Models;
using Sellorio.AudioOracle.ServiceInterfaces.Search;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Services.Search;

internal class SearchService(DatabaseContext databaseContext, IProviderInvocationService providerInvocationService, IEnumerable<IDownloadProvider> downloadProviders) : ISearchService
{
    public async Task<ValueResult<IList<SearchResult>>> SearchAsync(MetadataSearchPost search)
    {
        var providersSearchResult =
            await providerInvocationService.InvokeAsync<IMetadataSearchProvider, PagedList<MetadataSearchResult>>(
                search.IncludedProviders,
                x => x.SearchForMetadataAsync(search.SearchText, pageSize: 20));

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

    public async Task<ValueResult<IList<Models.Search.DownloadSearchResult>>> SearchForDownloadAsync(int trackId)
    {
        var track =
            await databaseContext.Tracks
                .AsNoTracking()
                .Include(x => x.Album)
                .Include(x => x.Artists)!.ThenInclude(x => x.Artist)
                .SingleOrDefaultAsync(x => x.Id == trackId);

        if (track == null)
        {
            return ResultMessage.NotFound("Track");
        }

        var criteria = new DownloadSearchCriteria
        {
            TrackTitle = track.Title!,
            AlbumTitle = track.Album!.Title,
            MainArtist = track.Artists!.FirstOrDefault()?.Artist!.Name
        };

        var providersSearchResult =
            await providerInvocationService.InvokeAsync<IDownloadSearchProvider, PagedList<AudioOracle.Providers.Models.DownloadSearchResult>>(
                x => x.SearchForDownloadAsync(criteria, pageSize: 20));

        if (!providersSearchResult.WasSuccess)
        {
            return ValueResult<IList<Models.Search.DownloadSearchResult>>.Failure(providersSearchResult.Messages);
        }

        var results = new List<Models.Search.DownloadSearchResult>(providersSearchResult.Value!.Sum(x => x.Items.Count));

        foreach (var providerResults in providersSearchResult.Value!)
        {
            foreach (var providerResult in providerResults.Items)
            {
                results.Add(new()
                {
                    AlbumArtUrl = providerResult.AlbumArtUrl,
                    AlbumTitle = providerResult.AlbumTitle,
                    ArtistNames = providerResult.ArtistNames,
                    DownloadSource = new()
                    {
                        Source = providerResult.Source,
                        SourceId = providerResult.Ids.SourceId,
                        SourceUrlId = providerResult.Ids.SourceUrlId
                    },
                    Title = providerResult.Title
                });
            }
        }

        return ValueResult<IList<Models.Search.DownloadSearchResult>>.Success(results, providersSearchResult.Messages);
    }

    public async Task<ValueResult<DownloadSource>> SearchForDownloadByUrlAsync(string url)
    {
        var isSupportedUrl = false;
        Result? failedResult = null;

        foreach (var downloadProvider in downloadProviders)
        {
            if (downloadProvider.IsSupportedDownloadUrl(url))
            {
                isSupportedUrl = true;

                var downloadIdsResult = await downloadProvider.ResolveIdsFromTrackUrlAsync(url);

                if (downloadIdsResult.WasSuccess)
                {
                    return new DownloadSource
                    {
                        Source = downloadProvider.ProviderName,
                        SourceId = downloadIdsResult.Value.SourceId,
                        SourceUrlId = downloadIdsResult.Value.SourceUrlId
                    };
                }
                else
                {
                    failedResult = downloadIdsResult.AsResult();
                }
            }
        }

        if (!isSupportedUrl)
        {
            return ResultMessage.Error(
                "The given url is not supported by any provider. " +
                "URL formats may be strict - try removing any unnecessary url parameters for example.");
        }

        return ValueResult<DownloadSource>.Failure(failedResult!.Messages);
    }
}
