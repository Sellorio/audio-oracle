using System.Threading.Tasks;
using Sellorio.AudioOracle.Client.Sessions;

namespace Sellorio.AudioOracle.Web.Client.Services;

public class SessionTokenProvider(ILocalStorageService localStorageService) : IAudioOracleSessionTokenProvider
{
    private const string LocalStorageKey = "AOToken";

    private bool _isLoaded;
    private string? _token;

    public async Task<string?> GetSessionTokenAsync()
    {
        if (!_isLoaded)
        {
            _token = await localStorageService.GetItemAsync(LocalStorageKey);
            _isLoaded = true;
        }

        return _token;
    }

    public async Task SetSessionTokenAsync(string? sessionToken)
    {
        _token = sessionToken;

        if (sessionToken == null)
        {
            await localStorageService.RemoveItemAsync(LocalStorageKey);
        }
        else
        {
            await localStorageService.SetItemAsync(LocalStorageKey, sessionToken);
        }
    }
}
