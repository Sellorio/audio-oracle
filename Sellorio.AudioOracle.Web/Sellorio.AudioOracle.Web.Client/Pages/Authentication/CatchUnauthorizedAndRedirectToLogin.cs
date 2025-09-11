using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Sellorio.AudioOracle.Client.Exceptions;
using Sellorio.AudioOracle.Web.Client.Services;

namespace Sellorio.AudioOracle.Web.Client.Pages.Authentication;

public class CatchUnauthorizedAndRedirectToLogin : ErrorBoundary
{
    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    [Inject]
    public required AuthenticationStateProvider AuthenticationStateProvider { get; init; }

    protected override Task OnErrorAsync(Exception exception)
    {
        if (exception is UnauthorizedException)
        {
            AuthenticationStateProvider.InvalidateAuthenticationState();
            //NavigationManager.NavigateTo(NavigationManager.BaseUri + "login?redirectUrl=" + WebUtility.UrlEncode(NavigationManager.Uri));
        }
        else
        {
            ExceptionDispatchInfo.Throw(exception);
        }

        return Task.CompletedTask;
    }
}
