using System;
using System.Net.Mime;
using System.Text.Json.Serialization;

namespace Sellorio.AudioOracle.Models.Content;

public class FileInfo
{
    public required int Id { get; init; }
    public required string UrlId { get; init; }
    public required FileType Type { get; init; }
    public required int Size { get; init; }
    public required FileContent? Content { get; init; }

    [JsonIgnore]
    public string MimeType => Type switch
    {
        FileType.ImageJpeg => MediaTypeNames.Image.Jpeg,
        FileType.ImagePng => MediaTypeNames.Image.Png,
        FileType.Unspecified => MediaTypeNames.Application.Octet,
        _ => throw new NotSupportedException("Unexpected file type.")
    };
}
