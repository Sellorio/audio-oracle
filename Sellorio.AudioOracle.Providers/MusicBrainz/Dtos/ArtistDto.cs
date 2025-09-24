using System;
using System.Collections.Generic;

namespace Sellorio.AudioOracle.Providers.MusicBrainz.Dtos;

internal class ArtistDto
{
    public required Guid Id { get; init; }
    public string? Type { get; init; }
    public string? Country { get; init; }
    public required string Name { get; init; }
    public string? Gender { get; init; }
    public required string SortName { get; init; }
    // this is used for fictional characters credited as song artists (may be used for other things too though)
    public string? Disambiguation { get; init; }
    public AreaDto? Area { get; init; }
    public IList<ArtistAliasDto>? Aliases { get; init; }
}
