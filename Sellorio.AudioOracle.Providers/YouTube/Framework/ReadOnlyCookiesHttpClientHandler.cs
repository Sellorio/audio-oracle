using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Providers.YouTube.Framework;

internal class ReadOnlyCookiesHttpClientHandler : HttpClientHandler
{
    public ReadOnlyCookiesHttpClientHandler()
    {
        UseCookies = false;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.TryAddWithoutValidation("Cookie", CookieContainer?.GetCookieHeader(request.RequestUri!));
        var result = await base.SendAsync(request, cancellationToken);
        return result;
    }

    private static string ToCookieHeader(CookieCollection? cookies)
    {
        if (cookies == null || cookies.Count == 0)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        var first = true;

        foreach (Cookie cookie in cookies)
        {
            if (!first)
            {
                sb.Append("; ");
            }

            first = false;

            // Use Uri escaping rules for cookie values
            var name = cookie.Name;
            var value = Uri.EscapeDataString(cookie.Value ?? string.Empty);

            sb.Append(name).Append('=').Append(value);
        }

        return sb.ToString();
    }
}
