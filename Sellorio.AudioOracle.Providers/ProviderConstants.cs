namespace Sellorio.AudioOracle.Providers;

internal static class ProviderConstants
{
    public static string UserAgent { get; }

    static ProviderConstants()
    {
        var assemblyVersion = typeof(ProviderConstants).Assembly.GetName().Version;
        UserAgent = $"sellorio-audio-oracle/{assemblyVersion.Major}.{assemblyVersion.Minor}";
    }
}
