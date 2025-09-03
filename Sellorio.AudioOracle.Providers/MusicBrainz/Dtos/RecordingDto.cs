using System;
using System.Collections.Generic;

namespace Sellorio.AudioOracle.Providers.MusicBrainz.Dtos;

internal class RecordingDto
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required int? Length { get; init; }
    public required IList<ReleaseDto> Releases { get; init; }
    public required IList<ArtistCreditItemDto> ArtistCredit { get; init; }
    public required int? Score { get; init; }
}
