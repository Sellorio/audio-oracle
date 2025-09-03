using System.Collections.Generic;
using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Metadata;
using Sellorio.AudioOracle.Providers.Models;

namespace Sellorio.AudioOracle.Services.Metadata;
internal interface IArtistCreationService
{
    Task<ValueResult<IList<Artist>>> GetOrCreateArtistsAsync(string providerName, IList<ResolvedIds> resolvedIds);
}