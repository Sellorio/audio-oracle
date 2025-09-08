using System.ComponentModel.DataAnnotations;

namespace Sellorio.AudioOracle.Data.Metadata;

public class ArtistNameData
{
    public int Id { get; init; }

    [Required]
    public int? ArtistId { get; set; }
    public ArtistData? Artist { get; set; }

    [Required, StringLength(Models.Metadata.Artist.NameMaxLength)]
    public required string Name { get; set; }
}
