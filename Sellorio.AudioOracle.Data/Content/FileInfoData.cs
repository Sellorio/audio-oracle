using Sellorio.AudioOracle.Models.Content;
using System.ComponentModel.DataAnnotations;

namespace Sellorio.AudioOracle.Data.Content
{
    public class FileInfoData
    {
        public int Id { get; set; }

        [Required]
        public int? ContentId { get; set; }
        public FileContentData Content { get; set; }

        [Required, StringLength(5)]
        public string UrlId { get; set; }

        [Required]
        public FileType? Type { get; set; }

        [Required]
        public int? Size { get; set; }
    }
}
