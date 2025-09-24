using System;
using System.Collections.Generic;

namespace Sellorio.AudioOracle.Providers.MusicBrainz.Dtos;

internal class TrackDto
{
    public required Guid Id { get; init; }
    public required string Number { get; set; }
    public required string Title { get; init; }
    public int? Length { get; init; }
    public TrackRecordingDto? Recording { get; init; }
    public IList<ArtistCreditItemDto>? ArtistCredit { get; init; }
}
