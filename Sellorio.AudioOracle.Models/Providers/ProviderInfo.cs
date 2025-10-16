namespace Sellorio.AudioOracle.Models.Providers;

public class ProviderInfo
{
    public required string Name { get; init; }
    public required bool IsMetadataSource { get; init; }
    public required bool IsDownloadSource { get; init; }
}
