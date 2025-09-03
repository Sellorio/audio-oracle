using System.Collections.Generic;

namespace Sellorio.AudioOracle.Models.Metadata;

public class Artist
{
    public const int NameMaxLength = 200;

    public required int Id { get; init; }
    public required ArtistType Type { get; init; }
    public required string Name { get; init; }
    public required ArtistGender? Gender { get; init; }
    public required string CountryCode { get; init; }
    public required string Country { get; init; }
    public required IList<ArtistName> ArtistNames { get; init; }
}
