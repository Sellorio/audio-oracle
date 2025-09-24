namespace Sellorio.AudioOracle.Providers.MusicBrainz.Dtos;

internal class ArtistCreditItemDto
{
    public string? Joinphrase { get; init; }
    public required string Name { get; init; }
    public required ArtistDto Artist { get; init; }
}
