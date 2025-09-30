using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Search;
using Sellorio.AudioOracle.Providers.Models;
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

internal class MetadataSearchProvider(HttpClient httpClient, ICoverArtArchiveService coverArtArchiveService) : IMetadataSearchProvider
{
    public string ProviderName => Constants.ProviderName;

    public async Task<ValueResult<PagedList<MetadataSearchResult>>> SearchForMetadataAsync(string searchText, int pageSize)
    {
        var recordingsSearchResult = await SearchRecordingsAsync(searchText, pageSize);
        var releasesSearchResult = await SearchReleasesAsync(searchText, pageSize);

        var recordingCount = recordingsSearchResult.Recordings.Count;
        var releaseCount = releasesSearchResult.Releases.Count;

        var recordingIndex = 0;
        var releaseIndex = 0;

        var searchResults = new List<MetadataSearchResult>(pageSize);

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

        return new PagedList<MetadataSearchResult>
        {
            Items = searchResults,
            Page = 1,
            PageSize = pageSize
        };
    }

    private async Task<RecordingsSearchResultDto> SearchRecordingsAsync(string searchText, int count)
    {
        RecordingsSearchResultDto? result = null;

        await RateLimiters.MusicBrainz.WithRateLimit(async () =>
        {
            var searchUri = $"recording/?fmt=json&limit={count}&offset=0&query={Uri.EscapeDataString(QueryHelper.EscapeValue(searchText))}";
            var response = await httpClient.GetAsync(searchUri);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            result = JsonSerializer.Deserialize<RecordingsSearchResultDto>(json, Constants.JsonOptions);
        });

        return result!;
    }

    private async Task<ReleasesSearchResultDto> SearchReleasesAsync(string searchText, int count)
    {
        ReleasesSearchResultDto? result = null;

        await RateLimiters.MusicBrainz.WithRateLimit(async () =>
        {
            var searchUri = $"release/?fmt=json&limit={count}&offset=0&query={Uri.EscapeDataString(QueryHelper.EscapeValue(searchText))}";
            var response = await httpClient.GetAsync(searchUri);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            result = JsonSerializer.Deserialize<ReleasesSearchResultDto>(json, Constants.JsonOptions)!;
        });

        return result!;
    }

    private async Task<MetadataSearchResult?> RecordingToSearchResultAsync(RecordingDto recording)
    {
        var releases = recording.Releases!.Where(ReleaseMinimumRequirements).OrderBy(GetReleasePreferenceOrder).ToArray();

        if (releases.Length == 0)
        {
            return null;
        }

        foreach (var release in releases)
        {
            var coverArtUrl = await coverArtArchiveService.GetReleaseArtUrlAsync(release.Id);

            if (coverArtUrl != null)
            {
                return new MetadataSearchResult
                {
                    AlbumTitle = release.Title,
                    AlternateAlbumTitle = null,
                    AlbumArtUrl = coverArtUrl,
                    AlbumIds = new() { SourceId = release.Id.ToString(), SourceUrlId = release.Id.ToString() },
                    Source = Constants.ProviderName,
                    Title = recording.Title,
                    AlternateTitle = null,
                    Type = SearchResultType.Track,
                    ArtistNames = release.ArtistCredit?.Select(x => x.Artist.Name).ToArray() ?? []
                };
            }
        }

        return new MetadataSearchResult
        {
            AlbumTitle = releases[0].Title,
            AlternateAlbumTitle = null,
            AlbumArtUrl = null,
            AlbumIds = new() { SourceId = releases[0].Id.ToString(), SourceUrlId = releases[0].Id.ToString() },
            Source = Constants.ProviderName,
            Title = recording.Title,
            AlternateTitle = null,
            Type = SearchResultType.Track,
            ArtistNames = recording.ArtistCredit?.Select(x => x.Artist.Name).ToArray() ?? []
        };
    }

    private async Task<MetadataSearchResult?> ReleaseToSearchResultAsync(ReleaseDto release)
    {
        if (!ReleaseMinimumRequirements(release))
        {
            return null;
        }

        var coverArtUrl = await coverArtArchiveService.GetReleaseArtUrlAsync(release.Id);

        return new MetadataSearchResult
        {
            AlbumTitle = release.Title,
            AlternateAlbumTitle = null,
            AlbumArtUrl = coverArtUrl,
            AlbumIds = new() { SourceId = release.Id.ToString(), SourceUrlId = release.Id.ToString() },
            Source = Constants.ProviderName,
            Title = release.Title,
            AlternateTitle = null,
            Type = SearchResultType.Album,
            ArtistNames = release.ArtistCredit?.Select(x => x.Artist.Name).ToArray() ?? []
        };
    }

    private int GetReleasePreferenceOrder(ReleaseDto release)
    {
        var result = 0;

        result += (release.Date?.Year ?? release.ReleaseYear ?? 9999) * 1000;   // asc release date, then
        result += release.Media!.Sum(x => x.TrackCount);                         // asc track count

        return result;
    }

    private bool ReleaseMinimumRequirements(ReleaseDto release)
    {
        return release.Media!.All(x => x.Format is "CD" or "Digital Media");
    }
}
