using Sellorio.AudioOracle.Providers.MusicBrainz.Dtos;
using System;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Providers.MusicBrainz;

internal interface IMusicBrainzAlbumMetadataProvider
{
    Task<Release> GetMusicBrainzReleaseAsync(Guid id);
}
