using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Providers.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Providers;

public interface IArtistMetadataProvider : IProvider
{
    Task<ValueResult<IList<ArtistMetadata?>>> GetArtistMetadataAsync(IList<string> ids);
}
