namespace Sellorio.AudioOracle.Providers.SoundCloud.Models;

internal class PublisherMetadataDto
{
    public required int Id { get; init; }
    public required string Urn { get; init; }
    public string? Artist { get; init; }
    public string? AlbumTitle { get; init; }
}
