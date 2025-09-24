using System;
using System.Collections.Generic;

namespace Sellorio.AudioOracle.Providers.MusicBrainz.Dtos;

internal class RecordingDto
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public int? Length { get; init; }
    public IList<ReleaseDto>? Releases { get; init; }
    public IList<ArtistCreditItemDto>? ArtistCredit { get; init; }
    public int? Score { get; init; }
}
