using Sellorio.AudioOracle.Data.Metadata;
using Sellorio.AudioOracle.Models.Metadata;

namespace Sellorio.AudioOracle.Services.Metadata
{
    public interface IMetadataMapper
    {
        Album Map(AlbumData from);
        Artist Map(ArtistData from);
        ArtistName Map(ArtistNameData from);
        Track Map(TrackData from);
    }
}