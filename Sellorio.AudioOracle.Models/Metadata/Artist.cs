using System.Collections.Generic;

namespace Sellorio.AudioOracle.Models.Metadata;

public class Artist
{
    public const int NameMaxLength = 200;

    public int Id { get; set; }
    public ArtistType Type { get; set; }
    public string Name { get; set; }
    public ArtistGender? Gender { get; set; }
    public string CountryCode { get; set; }
    public string Country { get; set; }
    public IList<ArtistName> ArtistNames { get; set; }
}
