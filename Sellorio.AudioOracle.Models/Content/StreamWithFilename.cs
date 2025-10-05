using System.IO;

namespace Sellorio.AudioOracle.Models.Content;

public class StreamWithFilename
{
    public required string FileName { get; init; }
    public required Stream Stream { get; init; }
}
