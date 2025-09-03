using System;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Providers.MusicBrainz.Services;

internal interface ICoverArtArchiveService
{
    Task<string?> GetReleaseArtUrlAsync(Guid musicBrainzReleaseId);
}