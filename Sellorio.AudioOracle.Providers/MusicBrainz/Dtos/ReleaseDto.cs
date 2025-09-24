using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sellorio.AudioOracle.Providers.MusicBrainz.Dtos;

internal class ReleaseDto
{
    public required Guid Id { get; init; }
    public string? Status { get; init; }
    public int? TrackCount { get; set; }
    public required string Title { get; init; }
    public string? Country { get; init; } // XW = Worldwide
    public IList<ArtistCreditItemDto>? ArtistCredit { get; init; }
    public IList<MediumDto>? Media { get; init; }

    [JsonIgnore]
    public DateOnly? Date => DateOnly.TryParse(DateString, out var result) ? result : null;
    [JsonIgnore]
    public int? ReleaseYear => Date?.Year ?? (int.TryParse(DateString, out var year) ? year : null);

    [JsonPropertyName("date")]
    public string? DateString { get; init; }

    public int? Score { get; init; }
}
