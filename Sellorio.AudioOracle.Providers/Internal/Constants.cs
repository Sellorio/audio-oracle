namespace Sellorio.AudioOracle.Providers.Internal;

public static class Constants
{
    // The internal provider is used for metadata that has been processed from multiple
    // sources and is now owned by AudioOracle. For example, when merging Artists from
    // multiple sources, the newly created Artist record belongs to the Internal provider.
    public const string ProviderName = "Internal";
}
