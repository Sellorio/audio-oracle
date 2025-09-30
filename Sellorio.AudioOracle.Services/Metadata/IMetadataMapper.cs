using Sellorio.AudioOracle.Data.Metadata;
using Sellorio.AudioOracle.Library.Mapping;
using Sellorio.AudioOracle.Models.Metadata;

namespace Sellorio.AudioOracle.Services.Metadata;

public interface IMetadataMapper : IMap<AlbumData, Album>, IMap<ArtistData, Artist>, IMap<ArtistNameData, ArtistName>, IMap<TrackData, Track>, IMap<AlbumArtistData, Artist>, IMap<TrackArtistData, Artist>
{
}