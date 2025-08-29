using System.Threading.Tasks;
using Sellorio.AudioOracle.Client.Internal;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Sessions;
using Sellorio.AudioOracle.ServiceInterfaces.Sessions;

namespace Sellorio.AudioOracle.Client.Sessions;

internal class AuthenticationService(IRestClient restClient, IAudioOracleSessionTokenProvider audioOracleSessionTokenProvider) : IAuthenticationService
{
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
}
