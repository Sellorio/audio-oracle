using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Search;
using Sellorio.AudioOracle.Providers.MusicBrainz.Dtos;
using Sellorio.AudioOracle.Providers.MusicBrainz.Helpers;
using Sellorio.AudioOracle.Providers.MusicBrainz.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Providers.MusicBrainz;

internal class SearchProvider(HttpClient httpClient, ICoverArtArchiveService coverArtArchiveService) : ISearchProvider
{
    public string SourceName => Constants.ProviderName;

    public async Task<ValueResult<PagedList<SearchResult>>> SearchAsync(string searchText, int pageSize)
    {
        var recordingsSearchResult = await SearchRecordingsAsync(searchText, pageSize);
        var releasesSearchResult = await SearchReleasesAsync(searchText, pageSize);

        var recordingCount = recordingsSearchResult.Count;
        var releaseCount = releasesSearchResult.Count;

        var recordingIndex = 0;
        var releaseIndex = 0;

        var searchResults = new List<SearchResult>(pageSize);

        while ((recordingIndex < recordingCount || releaseIndex < releaseCount) && searchResults.Count < pageSize)
        {
            var recordingScore = recordingIndex < recordingCount ? recordingsSearchResult.Recordings[recordingIndex].Score : 0;
            var releaseScore = releaseIndex < releaseCount ? releasesSearchResult.Releases[releaseIndex].Score : 0;

            if (releaseScore >= recordingScore)
            {
                var searchResult = await ReleaseToSearchResultAsync(releasesSearchResult.Releases[releaseIndex]);

                if (searchResult != null)
                {
                    searchResults.Add(searchResult);
                }

                releaseIndex++;
                continue;
            }
            else
            {
                var searchResult = await RecordingToSearchResultAsync(recordingsSearchResult.Recordings[recordingIndex]);

                if (searchResult != null)
                {
                    searchResults.Add(searchResult);
                }

                recordingIndex++;
                continue;
            }
        }

        return new PagedList<SearchResult>
        {
            Items = searchResults,
            Page = 1,
            PageSize = pageSize
        };
    }

    private async Task<RecordingsSearchResult> SearchRecordingsAsync(string searchText, int count)
    {
        RecordingsSearchResult result = null;

        await RateLimiters.MusicBrainz.WithRateLimit(async () =>
        {
            var searchUri = $"recording/?fmt=json&limit={count}&offset=0&query={Uri.EscapeDataString(QueryHelper.EscapeValue(searchText))}";
            var response = await httpClient.GetAsync(searchUri);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            result = JsonSerializer.Deserialize<RecordingsSearchResult>(json, Constants.JsonOptions);
        });

        return result;
    }

    private async Task<ReleasesSearchResult> SearchReleasesAsync(string searchText, int count)
    {
        ReleasesSearchResult result = null;

        await RateLimiters.MusicBrainz.WithRateLimit(async () =>
        {
            var searchUri = $"release/?fmt=json&limit={count}&offset=0&query={Uri.EscapeDataString(QueryHelper.EscapeValue(searchText))}";
            var response = await httpClient.GetAsync(searchUri);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            result = JsonSerializer.Deserialize<ReleasesSearchResult>(json, Constants.JsonOptions);
        });

        return result;
    }

    private async Task<SearchResult> RecordingToSearchResultAsync(Recording recording)
    {
        var releases = recording.Releases.Where(ReleaseMinimumRequirements).OrderBy(GetReleasePreferenceOrder).ToArray();

        if (releases.Length == 0)
        {
            return null;
        }

        foreach (var release in releases)
        {
            var coverArtUrl = await coverArtArchiveService.GetReleaseArtUrlAsync(release.Id);

            if (coverArtUrl != null)
            {
                return new SearchResult
                {
                    Album = release.Title,
                    AlbumArtUrl = coverArtUrl,
                    AlbumId = release.Id.ToString(),
                    AlbumUrlId = release.Id.ToString(),
                    Source = Constants.ProviderName,
                    Title = recording.Title,
                    Type = SearchResultType.Track
                };
            }
        }

        return new SearchResult
        {
            Album = releases[0].Title,
            AlbumArtUrl = null,
            AlbumId = releases[0].Id.ToString(),
            AlbumUrlId = releases[0].Id.ToString(),
            Source = Constants.ProviderName,
            Title = recording.Title,
            Type = SearchResultType.Track
        };
    }

    private async Task<SearchResult> ReleaseToSearchResultAsync(Release release)
    {
        if (!ReleaseMinimumRequirements(release))
        {
            return null;
        }

        var coverArtUrl = await coverArtArchiveService.GetReleaseArtUrlAsync(release.Id);

        return new SearchResult
        {
            Album = release.Title,
            AlbumArtUrl = coverArtUrl,
            AlbumId = release.Id.ToString(),
            AlbumUrlId = release.Id.ToString(),
            Source = Constants.ProviderName,
            Title = release.Title,
            Type = SearchResultType.Album
        };
    }

    private int GetReleasePreferenceOrder(Release release)
    {
        var result = 0;

        result += (release.Date?.Year ?? release.ReleaseYear ?? 9999) * 1000;   // asc release date, then
        result += release.Media.Sum(x => x.TrackCount);                         // asc track count

        return result;
    }

    private bool ReleaseMinimumRequirements(Release release)
    {
        return release.Media.All(x => x.Format is "CD" or "Digital Media");
    }
}
