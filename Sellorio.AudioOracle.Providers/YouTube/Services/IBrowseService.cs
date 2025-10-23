using Sellorio.AudioOracle.Providers.YouTube.Models;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Providers.YouTube.Services;
internal interface IBrowseService
{
    Task<AlbumBrowseBasicInfo> ResolveAlbumBasicInfoFromBrowseIdAsync(string browseId);
    Task<string> ResolveChannelIdFromBrowseIdAsync(string browseId);
}