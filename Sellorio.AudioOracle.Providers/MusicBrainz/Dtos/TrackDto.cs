using System;
using System.Collections.Generic;

namespace Sellorio.AudioOracle.Providers.MusicBrainz.Dtos;

internal class TrackDto
{
    public required Guid Id { get; init; }
    public required string Number { get; set; }
    public required string Title { get; init; }
    public required int? Length { get; init; }
    public required TrackRecordingDto? Recording { get; init; }
    public required IList<ArtistCreditItemDto> ArtistCredit { get; init; }
}
