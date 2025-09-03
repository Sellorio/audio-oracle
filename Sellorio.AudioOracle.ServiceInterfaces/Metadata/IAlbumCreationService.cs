using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Metadata;
using Sellorio.AudioOracle.Models.Search;

namespace Sellorio.AudioOracle.ServiceInterfaces.Metadata;

public interface IAlbumCreationService
{
    Task<ValueResult<Album>> CreateAlbumFromSearchResultAsync(SearchResult searchResult);
}