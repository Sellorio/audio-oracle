using System.Text.RegularExpressions;

namespace Sellorio.AudioOracle.Providers.SoundCloud;

internal static partial class Constants
{
    public const string ProviderName = "SoundCloud";

    // e.g. https://soundcloud.com/nzko-kmdo/kimi-no-sei-full-version-by-sexyzone-from-a-condition-called-love-shoujo-anime
    [GeneratedRegex(@"^https:\/\/soundcloud\.com\/([\w-]+\/[\w-]+)$")]
    public static partial Regex SoundCloudUrlRegex();
}
