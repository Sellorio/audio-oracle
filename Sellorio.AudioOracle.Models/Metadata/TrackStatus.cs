namespace Sellorio.AudioOracle.Models.Metadata;

public enum TrackStatus
{
    MissingMetadata,
    MetadataRetrievalFailed,
    NotRequested,
    Downloading,
    DownloadFailed,
    Imported,
    DeleteRequested
}
