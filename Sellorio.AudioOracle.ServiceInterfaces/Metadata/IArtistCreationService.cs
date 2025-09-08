using System.Collections.Generic;
using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Metadata;

namespace Sellorio.AudioOracle.ServiceInterfaces.Metadata;
public interface IArtistCreationService
{
    Task<ValueResult<IList<Artist?>>> GetOrCreateArtistsAsync(IList<ArtistPost> artistPosts);
}