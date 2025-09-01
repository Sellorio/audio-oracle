using Sellorio.AudioOracle.Models.Metadata;
using System.Collections.Generic;

namespace Sellorio.AudioOracle.Providers.Models;

public class ArtistMetadata
{
    public string Name { get; set; }
    public ArtistType Type { get; set; }
    public ArtistGender? Gender { get; set; }
    public string CountryCode { get; set; }
    public string Country { get; set; }
    public IList<string> Aliases { get; set; }
}
