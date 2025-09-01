using System;
using System.Collections.Generic;

namespace Sellorio.AudioOracle.Providers.MusicBrainz.Dtos;

internal class Track
{
    public Guid Id { get; set; }
    public string Number { get; set; }
    public string Title { get; set; }
    public int? Length { get; set; }
    public TrackRecording Recording { get; set; }
    public IList<ArtistCreditItem> ArtistCredit { get; set; }
}
