using System.Linq;
using System.Threading.Tasks;
using Sellorio.AudioOracle.Library;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Providers.Models;
using SoundCloudExplode;
using SoundCloudExplode.Search;

namespace Sellorio.AudioOracle.Providers.SoundCloud;

internal class DownloadSearchProvider(SoundCloudClient soundCloudClient) : IDownloadSearchProvider
{
    public string ProviderName => Constants.ProviderName;

    public async Task<ValueResult<PagedList<DownloadSearchResult>>> SearchForDownloadAsync(DownloadSearchCriteria searchCriteria, int pageSize)
    {
        var rawResults =
            await soundCloudClient.Search
                .GetTracksAsync(searchCriteria.TrackTitle + (searchCriteria.MainArtist == null ? "" : " " + searchCriteria.MainArtist))
                .Take(pageSize)
                .ToArrayAsync();

        var searchResults = rawResults.Select(Convert).ToArray();

        return new PagedList<DownloadSearchResult>
        {
            Items = searchResults,
            Page = 1,
            PageSize = pageSize
        };
    }

    private static DownloadSearchResult Convert(TrackSearchResult trackSearchResult)
    {
        var album = trackSearchResult.PlaylistName ?? trackSearchResult.User?.Username ?? "No Album";
        var artist = trackSearchResult.PublisherMetadata?.Artist ?? trackSearchResult.User?.FullName;

        return new DownloadSearchResult
        {
            AlbumArtUrl = trackSearchResult.ArtworkUrl?.AbsoluteUri,
            AlbumTitle = album,
            ArtistNames = artist == null ? [] : [artist],
            Ids = new() {  SourceId = trackSearchResult.Id.ToString(), SourceUrlId = Constants.SoundCloudUrlRegex().Match(trackSearchResult.Url!).Groups[1].Value },
            Source = Constants.ProviderName,
            Title = trackSearchResult.Title!
        };
    }
}
