using System.ComponentModel.DataAnnotations;

namespace Sellorio.AudioOracle.Models.Metadata;

public enum TrackStatus
{
    [Display(Name = "Fetching Metadata", Description = "Metadata for this track is being fetched from the metadata source now.")]
    MissingMetadata,
    [Display(Name = "Metadata Fetch Failed")]
    MetadataRetrievalFailed,
    [Display(Name = "Not Requested", Description = "The track has not been requested and will not be downloaded.")]
    NotRequested,
    [Display(Name = "Missing Download Source", Description = "The track has been requested but there is no download source. Please select a download source for the track.")]
    MissingDownloadSource,
    [Display(Name = "Downloading", Description = "The track is currently being downloaded.")]
    Downloading,
    [Display(Name = "Download Failed")]
    DownloadFailed,
    [Display(Name = "Imported", Description = "The track has been downloaded successfully and should be in the media library.")]
    Imported
}
