using Microsoft.Extensions.Caching.Memory;
using Sellorio.AudioOracle.Library.ApiTools;
using System;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Providers;

internal static class ProviderHelper
{
    public static Task<TResult?> GetWithCacheAndRateLimitAsync<TKey, TResult>(MemoryCache cache, TimeSpan cacheDuration, RateLimiter rateLimit, TKey key, Func<TKey, Task<TResult>> getter)
    {
        if (key == null)
        {
            throw new ArgumentException("Must not be null.", nameof(key));
        }

        var lazyTask = cache.GetOrCreate(key, (entry) =>
        {
            entry.AbsoluteExpirationRelativeToNow = cacheDuration;

            return
                new Lazy<Task<TResult?>>(async () =>
                {
                    TResult? result = default;
                    await rateLimit.WithRateLimit(async () => await getter?.Invoke(key)!);
                    return result;
                },
                isThreadSafe: true);
        })!;

        return lazyTask.Value;
    }

    public static Task<TResult> GetWithCacheAsync<TKey, TResult>(MemoryCache cache, TimeSpan cacheDuration, TKey key, Func<TKey, Task<TResult>> getter)
    {
        var lazyTask = cache.GetOrCreate(key, (entry) =>
        {
            entry.AbsoluteExpirationRelativeToNow = cacheDuration;

            return
                new Lazy<Task<TResult>>(() => getter?.Invoke(key),
                isThreadSafe: true);
        });

        return lazyTask.Value;
    }
}
