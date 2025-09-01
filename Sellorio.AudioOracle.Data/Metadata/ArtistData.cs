using Sellorio.AudioOracle.Models.Metadata;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sellorio.AudioOracle.Data.Metadata;

public class ArtistData
{
    public int Id { get; set; }

    [Required]
    public string Source { get; set; }

    [Required, StringLength(Album.SourceUrlIdMaxLength)]
    public string SourceUrlId { get; set; }

    [Required, StringLength(Album.SourceIdMaxLength)]
    public string SourceId { get; set; }

    public ArtistType? Type { get; set; }

    [StringLength(Artist.NameMaxLength)]
    public string Name { get; set; }

    public ArtistGender? Gender { get; set; }

    [StringLength(5)]
    public string CountryCode { get; set; }

    [StringLength(150)]
    public string Country { get; set; }

    public IList<ArtistNameData> ArtistNames { get; set; }
}
