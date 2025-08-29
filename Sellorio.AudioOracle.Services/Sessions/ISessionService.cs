using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;

namespace Sellorio.AudioOracle.Services.Sessions;

public interface ISessionService
{
    Task<Result> InvalidateAllSessionsAsync();
    Task<bool> IsCorrectPasswordAsync(string password);
    Task<ValueResult<string>> LoginAsync(string password);
    Task<Result> LogoutAsync();
    Task<Result> UseSessionAsync(string sessionToken);
}