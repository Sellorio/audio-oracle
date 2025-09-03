using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sellorio.AudioOracle.Providers.MusicBrainz.Dtos;

internal class ReleaseDto
{
    public required Guid Id { get; init; }
    public required string Status { get; init; }
    public required int TrackCount { get; set; }
    public required string Title { get; init; }
    public required string Country { get; init; } // XW = Worldwide
    public required IList<ArtistCreditItemDto> ArtistCredit { get; init; }
    public required IList<MediumDto> Media { get; init; }

    [JsonIgnore]
    public DateOnly? Date => DateOnly.TryParse(DateString, out var result) ? result : null;
    [JsonIgnore]
    public int? ReleaseYear => Date?.Year ?? (int.TryParse(DateString, out var year) ? year : null);

    [JsonPropertyName("date")]
    public required string? DateString { get; init; }

    public required int? Score { get; init; }
}
