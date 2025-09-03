using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Sellorio.AudioOracle.Models.Metadata;

namespace Sellorio.AudioOracle.Data.Metadata;

[Index(nameof(Source), nameof(SourceId), IsUnique = true)]
public class ArtistData
{
    public int Id { get; set; }

    [Required]
    public required string Source { get; set; }

    [Required, StringLength(Album.SourceUrlIdMaxLength)]
    public required string SourceUrlId { get; set; }

    [Required, StringLength(Album.SourceIdMaxLength)]
    public required string SourceId { get; set; }

    public required ArtistType? Type { get; set; }

    [StringLength(Artist.NameMaxLength)]
    public required string? Name { get; set; }

    public required ArtistGender? Gender { get; set; }

    [StringLength(5)]
    public required string? CountryCode { get; set; }

    [StringLength(150)]
    public required string? Country { get; set; }

    public IList<ArtistNameData>? ArtistNames { get; set; }
}
