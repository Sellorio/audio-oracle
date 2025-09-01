using System.Collections.Generic;

namespace Sellorio.AudioOracle.Providers.MusicBrainz.Dtos
{
    internal class ReleasesSearchResult
    {
        public int Count { get; set; }
        public int Offset { get; set; }
        public IList<Release> Releases { get; set; }
    }
}
