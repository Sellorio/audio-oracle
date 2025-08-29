using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Client.Sessions;

public interface IAudioOracleSessionTokenProvider
{
    Task SetSessionTokenAsync(string sessionToken);
    Task<string> GetSessionTokenAsync();
}
