namespace Sellorio.AudioOracle.Providers.SoundCloud.Models;

internal class TranscodingDto
{
    public required string Url { get; init; }
    public required string Preset { get; init; }
    public required TranscodingFormatDto Format { get; init; }
}
