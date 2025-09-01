namespace Sellorio.AudioOracle.Providers.MusicBrainz.Dtos;

internal class RecordingMatch
{
    public Recording Recording { get; set; }
    public Release Release { get; set; }
    public ReleaseGroup ReleaseGroup { get; set; }
    public Medium Medium { get; set; }
    public Track Track { get; set; }
}
