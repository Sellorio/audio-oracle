using Microsoft.Extensions.Logging;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Library.Results.Messages;
using Sellorio.AudioOracle.Models.Search;
using Sellorio.AudioOracle.Providers;
using Sellorio.AudioOracle.ServiceInterfaces.Search;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Services.Search;

internal class SearchService(ILogger<SearchService> logger, IEnumerable<ISearchProvider> searchProviders) : ISearchService
{
    public async Task<ValueResult<IList<SearchResult>>> SearchAsync(string searchText)
    {
        var messages = new List<ResultMessage>();
        var result = new List<SearchResult>();

        foreach (var searchProvider in searchProviders)
        {
            try
            {
                var searchResultsResult = await searchProvider.SearchAsync(searchText, 1, 20);
                result.AddRange(searchResultsResult.Value.Items);
            }
            catch (Exception ex)
            {
                messages.Add(ResultMessage.Warning($"Failure when searching {searchProvider.SourceName}. See logs for more info."));
                logger.LogException(ex, "Failure when searching {SourceName}.", searchProvider.SourceName);
            }
        }

        return ValueResult<IList<SearchResult>>.Success(result, messages);
    }
}
