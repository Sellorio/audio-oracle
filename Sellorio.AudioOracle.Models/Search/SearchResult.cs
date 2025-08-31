namespace Sellorio.AudioOracle.Models.Search;

public class SearchResult
{
    public SearchResultType Type { get; set; }
    public string Source { get; set; }

    public string AlbumUrlId { get; set; }
    public string AlbumId { get; set; }

    public string Title { get; set; }
    public string Album { get; set; }
    public string AlbumArtUrl { get; set; }
}
