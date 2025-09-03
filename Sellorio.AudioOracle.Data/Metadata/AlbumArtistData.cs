using System.ComponentModel.DataAnnotations;

namespace Sellorio.AudioOracle.Data.Metadata;

public class AlbumArtistData
{
    public int Id { get; set; }

    [Required]
    public int? ArtistId { get; set; }
    public ArtistData? Artist { get; set; }

    [Required]
    public int? AlbumId { get; set; }
    public AlbumData? Album { get; set; }
}
