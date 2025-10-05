using System.Linq;
using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Providers.Models;
using Sellorio.AudioOracle.Providers.SoundCloud.Models;
using Sellorio.AudioOracle.Providers.SoundCloud.Services;

namespace Sellorio.AudioOracle.Providers.SoundCloud;

internal class DownloadSearchProvider(ISoundCloudApiService soundCloudApiService) : IDownloadSearchProvider
{
    public string ProviderName => Constants.ProviderName;

    public async Task<ValueResult<PagedList<DownloadSearchResult>>> SearchForDownloadAsync(DownloadSearchCriteria searchCriteria, int pageSize)
    {
        var searchResult = await soundCloudApiService.SearchTracksAsync(searchCriteria.TrackTitle);
        var searchResults = searchResult.Collection.Select(Convert).ToArray();

        return new PagedList<DownloadSearchResult>
        {
            Items = searchResults,
            Page = 1,
            PageSize = pageSize
        };
    }

    private static DownloadSearchResult Convert(TrackDto trackSearchResult)
    {
        var album = trackSearchResult.PublisherMetadata?.AlbumTitle ?? trackSearchResult.Genre ?? "No Album";
        var artist = trackSearchResult.PublisherMetadata?.Artist ?? trackSearchResult.User?.FullName;

        return new DownloadSearchResult
        {
            AlbumArtUrl = trackSearchResult.ArtworkUrl,
            AlbumTitle = album,
            ArtistNames = artist == null ? [] : [artist],
            Ids = new() {  SourceId = trackSearchResult.Id.ToString(), SourceUrlId = Constants.SoundCloudUrlRegex().Match(trackSearchResult.PermalinkUrl!).Groups[1].Value },
            Source = Constants.ProviderName,
            Title = trackSearchResult.Title!
        };
    }
}
