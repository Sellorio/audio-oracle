using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Providers.Common;

internal class HttpClientHandlerWithCookies<TCacheScope> : HttpClientHandler
{
    private static readonly object _lock = new();
    private static readonly TimeSpan _checkFrequency = TimeSpan.FromSeconds(5);
    private static DateTime _cookieFileLastModified;
    private static DateTime _cookieFileLastChecked;
    private readonly string _cookiesFilePath;

    public HttpClientHandlerWithCookies(string cookiesFilePath)
    {
        _cookiesFilePath = cookiesFilePath;
        AutomaticDecompression = DecompressionMethods.All;
    }

    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        CheckAndUpdateCookies();
        return base.Send(request, cancellationToken);
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        CheckAndUpdateCookies();
        return base.SendAsync(request, cancellationToken);
    }

    private void CheckAndUpdateCookies()
    {
        var now = DateTime.UtcNow;

        if (_cookieFileLastChecked == default || now - _cookieFileLastChecked > _checkFrequency)
        {
            lock (_lock)
            {
                if (_cookieFileLastChecked == default || now - _cookieFileLastChecked > _checkFrequency)
                {
                    var newLastModified = File.GetLastWriteTimeUtc(_cookiesFilePath);

                    if (newLastModified != _cookieFileLastModified)
                    {
                        var cookieContainer = new CookieContainer();

                        foreach (var line in File.ReadLines(_cookiesFilePath))
                        {
                            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                                continue; // skip comments and empty lines

                            // Format: domain \t flag \t path \t secure \t expiry \t name \t value
                            var parts = line.Split('\t');
                            if (parts.Length != 7)
                                continue; // skip invalid lines

                            string domain = parts[0];
                            string path = parts[2];
                            bool secure = parts[3].Equals("TRUE", StringComparison.OrdinalIgnoreCase);
                            string name = parts[5];
                            string value = parts[6];

                            cookieContainer.Add(new Cookie(name, value, path, domain.TrimStart('.')));
                        }

                        CookieContainer = cookieContainer;
                        _cookieFileLastModified = newLastModified;
                    }

                    _cookieFileLastChecked = now;
                }
            }
        }
    }
}
