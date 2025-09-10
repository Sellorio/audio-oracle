using System;
using System.Threading.Tasks;
using Sellorio.AudioOracle.Client.Exceptions;
using Sellorio.AudioOracle.Client.Internal;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Sessions;
using Sellorio.AudioOracle.ServiceInterfaces.Sessions;

namespace Sellorio.AudioOracle.Client.Sessions;

internal class SessionService(IRestClient restClient, IAudioOracleSessionTokenProvider audioOracleSessionTokenProvider) : ISessionService
{
    public Task<Result> InvalidateAllSessionsAsync()
    {
        throw new NotSupportedException("This action is not valid when running in Client-Side Blazor.");
    }

    public Task<bool> IsCorrectPasswordAsync(string password)
    {
        throw new NotSupportedException("This action is not valid when running in Client-Side Blazor.");
    }

    public async Task<ValueResult<bool>> IsLoggedInAsync()
    {
        try
        {
            var result = await restClient.Get($"/sessions/ping").ToResult();
            return result.WasSuccess;
        }
        catch (UnauthorizedException)
        {
            return false;
        }
    }

    public async Task<ValueResult<string>> LoginAsync(string password)
    {
        var result = await restClient.Post($"/sessions/login", new LoginPost { Password = password }).ToValueResult<string>();

        if (result.WasSuccess)
        {
            await audioOracleSessionTokenProvider.SetSessionTokenAsync(result.Value);
        }

        return result;
    }

    public async Task<Result> LogoutAsync()
    {
        var result = await restClient.Post($"/sessions/logout").ToResult();

        if (result.WasSuccess)
        {
            await audioOracleSessionTokenProvider.SetSessionTokenAsync(null);
        }

        return result;
    }

    public Task<Result> UseSessionAsync(string sessionToken)
    {
        throw new NotSupportedException("This action is not valid when running in Client-Side Blazor.");
    }
}
