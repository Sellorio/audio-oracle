using Sellorio.AudioOracle.Data.Content;
using Sellorio.AudioOracle.Data.Metadata;
using Sellorio.AudioOracle.Library.Mapping;
using Sellorio.AudioOracle.Models.Content;
using Sellorio.AudioOracle.Models.Metadata;
using Sellorio.AudioOracle.Services.Content;

namespace Sellorio.AudioOracle.Services.Metadata;

internal class MetadataMapper(IContentMapper contentMapper) : StaticMapperBase<MetadataMapper>, IMetadataMapper
{
    protected override void Configure(MapMethodConfiguration<MetadataMapper> configure)
    {
        configure.AddWithConfig<AlbumArtistData, Artist?>(o =>
        {
            o.ConstructUsing(o => o.Artist == null ? null : Map(o.Artist));
            o.ForAllMembers(o => o.Ignore());
        });

        configure.AddWithConfig<TrackArtistData, Artist?>(o =>
        {
            o.ConstructUsing(o => o.Artist == null ? null : Map(o.Artist));
            o.ForAllMembers(o => o.Ignore());
        });

        configure.AddWithConfig<FileInfoData, FileInfo>(o => o.ConstructUsing(x => contentMapper.Map(x)));
    }

    public Album Map(AlbumData from) => Map<Album>(from);
    public Artist Map(ArtistData from) => Map<Artist>(from);
    public ArtistName Map(ArtistNameData from) => Map<ArtistName>(from);
    public Track Map(TrackData from) => Map<Track>(from);
    public Artist? Map(AlbumArtistData from) => Map<Artist?>(from);
    public Artist? Map(TrackArtistData from) => Map<Artist?>(from);
}
