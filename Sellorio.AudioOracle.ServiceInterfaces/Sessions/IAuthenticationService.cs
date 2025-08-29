using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;

namespace Sellorio.AudioOracle.ServiceInterfaces.Sessions;

public interface IAuthenticationService
{
    Task<ValueResult<string>> LoginAsync(string password);
    Task<Result> LogoutAsync();
}
