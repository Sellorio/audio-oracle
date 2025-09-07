using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;

namespace Sellorio.AudioOracle.Providers.YouTube.Services;

internal interface ITrackIdsResolver
{
    Task<ValueResult<string>> GetLatestIdAsync(string videoId);
}
