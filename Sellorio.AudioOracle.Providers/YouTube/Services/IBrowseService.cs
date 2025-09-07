using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Providers.YouTube.Services;
internal interface IBrowseService
{
    Task<string> ResolveAlbumIdFromBrowseIdAsync(string browseId);
    Task<string> ResolveChannelIdFromBrowseIdAsync(string browseId);
}