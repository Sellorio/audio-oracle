using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Providers.Common;

internal class HttpClientHandlerWithCookies : HttpClientHandler
{
    private readonly CachedCookieProvider _cachedCookieProvider;

    public HttpClientHandlerWithCookies(CachedCookieProvider cachedCookieProvider)
    {
        _cachedCookieProvider = cachedCookieProvider;
        AutomaticDecompression = DecompressionMethods.All;
        CookieContainer = new();
    }

    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return SendAsync(request, cancellationToken).GetAwaiter().GetResult();
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return _cachedCookieProvider.WithCookiesAsync(this, () => base.SendAsync(request, cancellationToken));
    }
}
