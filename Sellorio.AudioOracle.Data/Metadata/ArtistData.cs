using Sellorio.AudioOracle.Models.Metadata;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sellorio.AudioOracle.Data.Metadata
{
    public class ArtistData
    {
        public int Id { get; set; }

        [Required]
        public Source? Source { get; set; }

        [Required, StringLength(Album.SourceUrlIdMaxLength)]
        public string SourceUrlId { get; set; }

        [Required, StringLength(Album.SourceIdMaxLength)]
        public string SourceId { get; set; }

        [Required, StringLength(Artist.NameMaxLength)]
        public string Name { get; set; }

        public IList<ArtistNameData> ArtistNames { get; set; }
    }
}
