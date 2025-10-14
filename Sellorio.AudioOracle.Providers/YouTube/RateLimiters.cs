using Sellorio.AudioOracle.Library.ApiTools;

namespace Sellorio.AudioOracle.Providers.YouTube;

internal static class RateLimiters
{
    public static RateLimiter YouTubeApi { get; } = new RateLimiter(200);
    public static RateLimiter YouTubeDlp { get; } = new RateLimiter(10_000);
    public static RateLimiter YouTubePage { get; } = new RateLimiter(3_000);
}
