using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Providers.Common;

internal class CachedCookieProvider(string cookieFilePath)
{
    private readonly AsyncReaderWriterLock _readerWriterLockSlim = new();
    private static readonly TimeSpan _checkFrequency = TimeSpan.FromSeconds(5);
    private DateTime? _cookieFileLastModified;
    private DateTime? _cookieFileLastChecked;
    private List<SavedCookie>? _savedCookies;

    public async Task<HttpResponseMessage> WithCookiesAsync(HttpClientHandler httpClientHandler, Func<Task<HttpResponseMessage>> action)
    {
        var now = DateTime.UtcNow;

        using (var upgradeableLock = await _readerWriterLockSlim.UpgradeableReadLockAsync())
        {
            if (_savedCookies == null ||
                _cookieFileLastChecked == null ||
                now - _cookieFileLastChecked > _checkFrequency)
            {
                var newLastModified = File.GetLastWriteTimeUtc(cookieFilePath);

                if (newLastModified != _cookieFileLastModified)
                {
                    using (await _readerWriterLockSlim.WriteLockAsync())
                    {
                        _savedCookies = [];

                        foreach (var line in File.ReadLines(cookieFilePath))
                        {
                            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                                continue; // skip comments and empty lines

                            // Format: domain \t flag \t path \t secure \t expiry \t name \t value
                            var parts = line.Split('\t');
                            if (parts.Length != 7)
                                continue; // skip invalid lines

                            string domain = parts[0];
                            string path = parts[2];
                            string name = parts[5];
                            string value = parts[6];

                            _savedCookies.Add(new SavedCookie { Name = name, Value = value, Path = path, Domain = domain.TrimStart('.') });
                        }

                        _cookieFileLastModified = newLastModified;
                    }
                }

                _cookieFileLastChecked = now;
            }

            var savedCookies = _savedCookies!;

            foreach (var existingCookie in httpClientHandler.CookieContainer.GetAllCookies().OfType<Cookie>())
            {
                existingCookie.Expired = true;
            }

            foreach (var newCookie in savedCookies)
            {
                httpClientHandler.CookieContainer.Add(new Cookie(newCookie.Name, newCookie.Value, newCookie.Path, newCookie.Domain));
            }

            return await action.Invoke();
        }
    }

    private class SavedCookie
    {
        public required string Name { get; set; }
        public required string? Value { get; set; }
        public required string? Path { get; set; }
        public required string? Domain { get; set; }
    }
}
