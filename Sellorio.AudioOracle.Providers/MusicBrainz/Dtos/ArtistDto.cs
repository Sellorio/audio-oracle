using System;
using System.Collections.Generic;

namespace Sellorio.AudioOracle.Providers.MusicBrainz.Dtos;

internal class ArtistDto
{
    public required Guid Id { get; init; }
    public required string Type { get; init; }
    public required string Country { get; init; }
    public required string Name { get; init; }
    public required string? Gender { get; init; }
    public required string SortName { get; init; }
    // this is used for fictional characters credited as song artists (may be used for other things too though)
    public required string? Disambiguation { get; init; }
    public required AreaDto Area { get; init; }
    public required IList<ArtistAliasDto> Aliases { get; init; }
}
