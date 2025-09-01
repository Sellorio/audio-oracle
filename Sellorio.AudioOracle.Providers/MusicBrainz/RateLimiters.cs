using Sellorio.AudioOracle.Library.ApiTools;

namespace Sellorio.AudioOracle.Providers.MusicBrainz;

internal class RateLimiters
{
    public static RateLimiter MusicBrainz { get; } = new(1000);
    public static RateLimiter CoverArtArchive { get; } = new(0);
}
