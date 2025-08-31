using Sellorio.AudioOracle.Data.Content;
using Sellorio.AudioOracle.Models.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sellorio.AudioOracle.Data.Metadata;

public class AlbumData
{
    public int Id { get; set; }

    [Required]
    public int? AlbumArtId { get; set; }
    public FileInfoData AlbumArt { get; set; }

    [Required]
    public string Source { get; set; }

    [Required, StringLength(Album.SourceUrlIdMaxLength)]
    public string SourceUrlId { get; set; }

    [Required, StringLength(Album.SourceIdMaxLength)]
    public string SourceId { get; set; }

    [Required, StringLength(Album.TitleMaxLength)]
    public string Title { get; set; }

    [StringLength(Album.TitleMaxLength)]
    public string AlternateTitle { get; set; }

    [Required]
    public ushort? TrackCount { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    public ushort? ReleaseYear { get; set; }

    public IList<TrackData> Tracks { get; set; }
    public IList<AlbumArtistData> Artists { get; set; }
}
