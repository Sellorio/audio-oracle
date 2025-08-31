using System.ComponentModel.DataAnnotations;

namespace Sellorio.AudioOracle.Data.Metadata
{
    public class TrackArtistData
    {
        public int Id { get; set; }

        [Required]
        public int? ArtistId { get; set; }
        public ArtistData Artist { get; set; }

        [Required]
        public int? TrackId { get; set; }
        public TrackData Track { get; set; }
    }
}
