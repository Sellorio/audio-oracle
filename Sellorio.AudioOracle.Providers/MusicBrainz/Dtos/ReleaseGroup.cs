using System;

namespace Sellorio.AudioOracle.Providers.MusicBrainz.Dtos;

internal class ReleaseGroup
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string PrimaryType { get; set; }
}
