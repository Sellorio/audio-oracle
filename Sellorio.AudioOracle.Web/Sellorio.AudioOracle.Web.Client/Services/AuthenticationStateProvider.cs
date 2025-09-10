using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Sellorio.AudioOracle.ServiceInterfaces.Sessions;

namespace Sellorio.AudioOracle.Web.Client.Services;

public class AuthenticationStateProvider(ISessionService sessionService) : Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider
{
    private static readonly ClaimsPrincipal _notLoggedIn = new(new ClaimsIdentity());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var isLoggedInResult = await sessionService.IsLoggedInAsync();

        if (!isLoggedInResult.WasSuccess)
        {
            throw new InvalidOperationException("Unexpected failure when checking log in state.");
        }

        return
            new AuthenticationState(
                isLoggedInResult.Value
                    ? new(new ClaimsIdentity("AOGuid"))
                    : _notLoggedIn);
    }

    public void InvalidateAuthenticationState()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
