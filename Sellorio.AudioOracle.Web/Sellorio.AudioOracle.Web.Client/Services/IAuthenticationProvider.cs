using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;

namespace Sellorio.AudioOracle.Web.Client.Services;

public interface IAuthenticationProvider
{
    Task<Result> LoginAsync(string password);
    Task<Result> LogoutAsync();
}