using Sellorio.AudioOracle.Models.Metadata;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sellorio.AudioOracle.Data.Metadata;

public class TrackData
{
    public int Id { get; set; }

    [Required]
    public int? AlbumId { get; set; }
    public AlbumData Album { get; set; }

    [Required]
    public string MetadataSource { get; set; }

    [Required, StringLength(Models.Metadata.Album.SourceUrlIdMaxLength)]
    public string MetadataSourceUrlId { get; set; }

    [Required, StringLength(Models.Metadata.Album.SourceIdMaxLength)]
    public string MetadataSourceId { get; set; }

    public string DownloadSource { get; set; }

    [StringLength(Models.Metadata.Album.SourceUrlIdMaxLength)]
    public string DownloadSourceUrlId { get; set; }

    [StringLength(Models.Metadata.Album.SourceIdMaxLength)]
    public string DownloadSourceId { get; set; }

    [StringLength(Models.Metadata.Album.TitleMaxLength)]
    public string Title { get; set; }

    [StringLength(Models.Metadata.Album.TitleMaxLength)]
    public string AlternateTitle { get; set; }

    public int? TrackNumber { get; set; }

    [Required]
    public bool? IsRequested { get; set; }

    public IList<TrackArtistData> Artists { get; set; }

    [Required]
    public TrackStatus? Status { get; set; }

    public string StatusText { get; set; }

    [StringLength(1000)]
    public string Filename { get; set; }
}
