using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.ServiceInterfaces.Sessions;

namespace Sellorio.AudioOracle.Services.Sessions;

internal class AuthenticationService(ISessionService sessionService) : IAuthenticationService
{
    public Task<ValueResult<string>> LoginAsync(string password)
    {
        return sessionService.LoginAsync(password);
    }

    public Task<Result> LogoutAsync()
    {
        return sessionService.LogoutAsync();
    }
}
