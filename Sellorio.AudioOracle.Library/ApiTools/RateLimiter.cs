using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Library.ApiTools;

public class RateLimiter
{
    private readonly TimeSpan _rateLimit;
    private readonly SemaphoreSlim _lock = new(1);
    private DateTime _lastAccessed;

    public RateLimiter(TimeSpan rateLimit)
    {
        _rateLimit = rateLimit;
    }

    public RateLimiter(int rateLimitMs)
    {
        _rateLimit = TimeSpan.FromMilliseconds(rateLimitMs);
    }

    public async Task WithRateLimit(Func<Task> func)
    {
        await _lock.WaitAsync();

        try
        {
            var waitTime = _lastAccessed == default ? TimeSpan.Zero : (_lastAccessed + _rateLimit - DateTime.UtcNow);

            if (waitTime > TimeSpan.Zero)
            {
                await Task.Delay(waitTime);
            }

            await func.Invoke();

            _lastAccessed = DateTime.UtcNow;
        }
        finally
        {
            _lock.Release();
        }
    }
}
