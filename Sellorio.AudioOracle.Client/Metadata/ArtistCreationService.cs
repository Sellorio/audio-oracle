using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sellorio.AudioOracle.Client.Internal;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Metadata;
using Sellorio.AudioOracle.ServiceInterfaces.Metadata;

namespace Sellorio.AudioOracle.Client.Metadata;

internal class ArtistCreationService(IRestClient restClient) : IArtistCreationService
{
    public async Task<ValueResult<IList<Artist?>>> GetOrCreateArtistsAsync(IList<ArtistPost> artistPosts)
    {
        return await restClient.Post($"/artists").ToValueResult<IList<Artist?>>();
    }
}
