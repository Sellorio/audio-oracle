using System;
using System.Collections.Generic;

namespace Sellorio.AudioOracle.Providers.MusicBrainz.Dtos;

internal class ArtistDto
{
    public Guid Id { get; set; }
    public string Type { get; set; }
    public string Country { get; set; }
    public string Name { get; set; }
    public string Gender { get; set; }
    public string SortName { get; set; }
    // this is used for fictional characters credited as song artists (may be used for other things too though)
    public string Disambiguation { get; set; }
    public Area Area { get; set; }
    public IList<ArtistAlias> Aliases { get; set; }
}
