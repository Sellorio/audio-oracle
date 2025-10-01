using System;
using System.Collections.Generic;
using Sellorio.AudioOracle.Models.Content;

namespace Sellorio.AudioOracle.Models.Metadata;

public class Track
{
    public required int Id { get; set; }
    public required int AlbumId { get; set; }
    public required int? AlbumArtOverrideId { get; set; }
    public required FileInfo? AlbumArtOverride { get; set; }
    public required bool IsRequested { get; set; } /* AutoMapper does not work for this property for some reason so it has to be settable */
    public required TrackStatus Status { get; set; }
    public required string StatusText { get; set; }
    public required string Filename { get; set; }

    public required string Title { get; set; }
    public required string AlternateTitle { get; set; }
    public required TimeSpan? Duration { get; set; }
    public required int? TrackNumber { get; set; }
    public required IList<Artist> Artists { get; set; }

    public required string MetadataSource { get; set; }
    public required string MetadataSourceUrlId { get; set; }
    public required string MetadataSourceId { get; set; }

    public string? DownloadSource { get; set; }
    public string? DownloadSourceUrlId { get; set; }
    public string? DownloadSourceId { get; set; }
}
