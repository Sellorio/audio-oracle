namespace Sellorio.AudioOracle.Providers.SoundCloud.Models;

internal class TrackDto
{
    public required long Id { get; init; }
    public required string ArtworkUrl { get; init; }
    public required int Duration { get; init; }
    public required int FullDuration { get; init; }
    public required string Title { get; init; }
    public string? Genre { get; init; }
    public required string PermalinkUrl { get; init; }
    public PublisherMetadataDto? PublisherMetadata { get; init; }
    public required UserDto User { get; init; }
    public required MediaDto Media { get; init; }
}
