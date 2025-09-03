using System.Collections.Generic;

namespace Sellorio.AudioOracle.Providers.MusicBrainz.Dtos;

internal class ReleaseArtImageDto
{
    public required bool Approved { get; init; }
    public required bool Back { get; init; }
    public required string Comment { get; init; }
    public required long Edit { get; init; }
    public required bool Front { get; init; }
    public required long Id { get; init; }
    public required string Image { get; init; }
    public required Dictionary<string, string> Thumbnails { get; init; }
    public required IList<string> Types { get; init; }
}
