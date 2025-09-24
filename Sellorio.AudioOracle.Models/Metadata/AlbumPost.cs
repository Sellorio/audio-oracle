using Sellorio.AudioOracle.Models.Search;

namespace Sellorio.AudioOracle.Models.Metadata;

public class AlbumPost
{
    public required SearchResult SearchResult { get; init; }
    public required TracksToRequest TracksToRequest { get; init; }
}
