using System.ComponentModel.DataAnnotations;

namespace Sellorio.AudioOracle.Data.Content;

public class FileContentData
{
    public int Id { get; set; }

    [Required]
    public required byte[] Data { get; set; }
}
