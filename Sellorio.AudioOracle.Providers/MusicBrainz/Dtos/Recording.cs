using System;
using System.Collections.Generic;

namespace Sellorio.AudioOracle.Providers.MusicBrainz.Dtos;

internal class Recording
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public int? Length { get; set; }
    public IList<Release> Releases { get; set; }
    public IList<ArtistCreditItem> ArtistCredit { get; set; }
    public int Score { get; set; }
}
