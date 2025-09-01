using Sellorio.AudioOracle.Providers.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Providers;

public interface IArtistMetadataProvider
{
    Task<IList<ArtistMetadata>> GetArtistMetadataAsync(string[] ids);
}
