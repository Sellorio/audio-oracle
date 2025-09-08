using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Sellorio.AudioOracle.Data.Content;
using Sellorio.AudioOracle.Models.Metadata;

namespace Sellorio.AudioOracle.Data.Metadata;

[Index(nameof(MetadataSource), nameof(MetadataSourceId), IsUnique = true)]
public class TrackData
{
    public int Id { get; init; }

    [Required]
    public int? AlbumId { get; set; }
    public AlbumData? Album { get; set; }

    public int? AlbumArtOverrideId { get; set; }
    public FileInfoData? AlbumArtOverride { get; set; }

    public required bool IsRequested { get; set; }

    public required TrackStatus Status { get; set; }

    [Required, StringLength(Models.Metadata.Album.SourceMaxLength)]
    public required string MetadataSource { get; set; }

    [Required, StringLength(Models.Metadata.Album.SourceUrlIdMaxLength)]
    public required string MetadataSourceUrlId { get; set; }

    [Required, StringLength(Models.Metadata.Album.SourceIdMaxLength)]
    public required string MetadataSourceId { get; set; }

    [StringLength(Models.Metadata.Album.SourceMaxLength)]
    public string? DownloadSource { get; set; }

    [StringLength(Models.Metadata.Album.SourceUrlIdMaxLength)]
    public string? DownloadSourceUrlId { get; set; }

    [StringLength(Models.Metadata.Album.SourceIdMaxLength)]
    public string? DownloadSourceId { get; set; }

    [StringLength(Models.Metadata.Album.TitleMaxLength)]
    public string? Title { get; set; }

    [StringLength(Models.Metadata.Album.TitleMaxLength)]
    public string? AlternateTitle { get; set; }

    public TimeSpan? Duration { get; set; }

    public int? TrackNumber { get; set; }

    [StringLength(300)]
    public string? StatusText { get; set; }

    [StringLength(1000)]
    public string? Filename { get; set; }

    public IList<TrackArtistData>? Artists { get; set; }
}
