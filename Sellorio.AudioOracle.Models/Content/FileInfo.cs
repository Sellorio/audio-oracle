namespace Sellorio.AudioOracle.Models.Content;

public class FileInfo
{
    public required int Id { get; init; }
    public required string UrlId { get; init; }
    public required FileType Type { get; init; }
    public required int Size { get; init; }
}
