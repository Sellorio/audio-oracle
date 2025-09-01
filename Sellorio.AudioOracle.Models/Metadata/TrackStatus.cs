namespace Sellorio.AudioOracle.Models.Metadata;

public enum TrackStatus
{
    MissingMetadata,
    MetadataRetrievalFailed,
    NotRequested,
    MissingDownloadSource,
    Downloading,
    DownloadFailed,
    Imported,
    DeleteRequested
}
