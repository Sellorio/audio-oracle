using Sellorio.AudioOracle.Library.ApiTools;

namespace Sellorio.AudioOracle.Providers.YouTube;

internal static class RateLimiters
{
    public static RateLimiter YouTubeApi { get; } = new RateLimiter(1_000);
    public static RateLimiter YouTubePageFetch { get; } = new RateLimiter(500);
    public static RateLimiter YouTubeDlp { get; } = new RateLimiter(10_000);
}
