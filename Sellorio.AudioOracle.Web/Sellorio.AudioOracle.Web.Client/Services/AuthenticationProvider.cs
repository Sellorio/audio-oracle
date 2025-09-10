using System.Threading.Tasks;
using Sellorio.AudioOracle.Client.Sessions;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.ServiceInterfaces.Sessions;

namespace Sellorio.AudioOracle.Web.Client.Services;

internal class AuthenticationProvider(ISessionService sessionService, IAudioOracleSessionTokenProvider sessionTokenProvider, AuthenticationStateProvider authenticationStateProvider) : IAuthenticationProvider
{
    public async Task<Result> LoginAsync(string password)
    {
        var loginResult = await sessionService.LoginAsync(password);

        if (loginResult.WasSuccess)
        {
            await sessionTokenProvider.SetSessionTokenAsync(loginResult.Value);
            authenticationStateProvider.InvalidateAuthenticationState();
        }

        return loginResult.AsResult();
    }

    public async Task<Result> LogoutAsync()
    {
        var logoutResult = await sessionService.LogoutAsync();

        if (logoutResult.WasSuccess)
        {
            await sessionTokenProvider.SetSessionTokenAsync(null);
            authenticationStateProvider.InvalidateAuthenticationState();
        }

        return logoutResult;
    }
}
