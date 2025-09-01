using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sellorio.AudioOracle.Providers.MusicBrainz;

internal static class Constants
{
    public const string ProviderName = "MusicBrainz";

    public static JsonSerializerOptions JsonOptions { get; }

    static Constants()
    {
        JsonOptions = new JsonSerializerOptions();
        JsonOptions.Converters.Add(new JsonStringEnumConverter());
        JsonOptions.PropertyNamingPolicy = JsonNamingPolicy.KebabCaseLower;
    }
}
