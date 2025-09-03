using Microsoft.EntityFrameworkCore;
using Sellorio.AudioOracle.Models.Content;
using System.ComponentModel.DataAnnotations;

namespace Sellorio.AudioOracle.Data.Content;

[Index(nameof(OriginalUrl), IsUnique = true)]
public class FileInfoData
{
    public int Id { get; set; }

    [Required]
    public int? ContentId { get; set; }
    public FileContentData? Content { get; set; }

    [Required, StringLength(5)]
    public required string UrlId { get; set; }

    public required FileType Type { get; set; }

    public required int Size { get; set; }

    public required string? OriginalUrl { get; set; }
}
