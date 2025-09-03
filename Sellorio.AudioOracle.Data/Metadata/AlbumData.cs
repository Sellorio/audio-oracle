using Microsoft.EntityFrameworkCore;
using Sellorio.AudioOracle.Data.Content;
using Sellorio.AudioOracle.Models.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sellorio.AudioOracle.Data.Metadata;

[Index(nameof(Source), nameof(SourceId), IsUnique = true)]
[Index(nameof(SourceId))]
public class AlbumData
{
    public int Id { get; set; }

    public int? AlbumArtId { get; set; }
    public FileInfoData? AlbumArt { get; set; }

    [Required]
    public required string Source { get; set; }

    [Required, StringLength(Album.SourceUrlIdMaxLength)]
    public required string SourceUrlId { get; set; }

    [Required, StringLength(Album.SourceIdMaxLength)]
    public required string SourceId { get; set; }

    [Required, StringLength(Album.TitleMaxLength)]
    public required string Title { get; set; }

    [StringLength(Album.TitleMaxLength)]
    public required string? AlternateTitle { get; set; }

    public required ushort TrackCount { get; set; }

    public required DateOnly? ReleaseDate { get; set; }

    public required ushort? ReleaseYear { get; set; }

    public IList<TrackData>? Tracks { get; set; }
    public IList<AlbumArtistData>? Artists { get; set; }
}
