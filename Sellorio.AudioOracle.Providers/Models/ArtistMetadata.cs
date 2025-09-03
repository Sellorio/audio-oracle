using Sellorio.AudioOracle.Models.Metadata;
using System.Collections.Generic;

namespace Sellorio.AudioOracle.Providers.Models;

public class ArtistMetadata
{
    public required string Name { get; init; }
    public required ArtistType Type { get; init; }
    public required ArtistGender? Gender { get; init; }
    public required string? CountryCode { get; init; }
    public required string? Country { get; init; }
    public required IList<string> Aliases { get; init; }
}
