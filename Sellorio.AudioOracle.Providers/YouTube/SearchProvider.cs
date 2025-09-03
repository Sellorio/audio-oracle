using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Search;
using Sellorio.AudioOracle.Providers.YouTube.Services;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Providers.YouTube;

internal class SearchProvider(IApiService apiService) : ISearchProvider
{
    private const string GetSongsParams = "EgWKAQIIAWoQEAMQBBAJEAoQBRAREBAQFQ%3D%3D";
    private const string GetAlbumsParams = "EgWKAQIYAWoQEAMQBBAJEAoQBRAREBAQFQ%3D%3D";

    public string SourceName => Constants.ProviderName;

    public async Task<ValueResult<PagedList<SearchResult>>> SearchAsync(string searchText, int pageSize)
    {
        var albumSearchResults = await SearchAlbumsAsync(searchText);
        var songSearchResults = await SearchSongsAsync(searchText);

        var albumsToTake =
            Math.Min(
                albumSearchResults.Length,
                (pageSize / 2) + ((pageSize / 2) - Math.Min(pageSize / 2, songSearchResults.Length)));

        var songsToTake = pageSize - albumsToTake;

        return new PagedList<SearchResult>
        {
            Items =
                Enumerable.Concat(
                    albumSearchResults.Take(albumsToTake),
                    songSearchResults.Take(songsToTake))
                        .ToArray(),
            Page = 1,
            PageSize = pageSize
        };
    }

    private async Task<SearchResult[]> SearchAlbumsAsync(string searchText)
    {
        var searchResponse = await apiService.GetAsync("/search?prettyPrint=false")
    }

    private Task<SearchResult[]> SearchSongsAsync(string searchText)
    {

    }
}
