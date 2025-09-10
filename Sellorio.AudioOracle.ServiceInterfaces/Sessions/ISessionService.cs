using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;

namespace Sellorio.AudioOracle.ServiceInterfaces.Sessions;

public interface ISessionService
{
    Task<ValueResult<bool>> IsLoggedInAsync();
    Task<Result> InvalidateAllSessionsAsync();
    Task<bool> IsCorrectPasswordAsync(string password);
    Task<ValueResult<string>> LoginAsync(string password);
    Task<Result> LogoutAsync();
    Task<Result> UseSessionAsync(string sessionToken);
}