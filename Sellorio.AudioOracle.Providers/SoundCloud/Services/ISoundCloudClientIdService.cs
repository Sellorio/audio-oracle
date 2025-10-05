using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Providers.SoundCloud.Services;

internal interface ISoundCloudClientIdService
{
    Task<string> GetClientIdAsync();
}