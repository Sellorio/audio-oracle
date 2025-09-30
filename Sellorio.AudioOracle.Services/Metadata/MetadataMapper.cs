using System;
using Sellorio.AudioOracle.Data.Metadata;
using Sellorio.AudioOracle.Library.Mapping;
using Sellorio.AudioOracle.Models.Metadata;
using Sellorio.AudioOracle.Services.Content;

namespace Sellorio.AudioOracle.Services.Metadata;

internal class MetadataMapper(IContentMapper contentMapper) : MapperBase(contentMapper), IMetadataMapper
{
    public Album Map(AlbumData from)
    {
        return Map<Album>(from);
    }

    public Artist Map(ArtistData from)
    {
        return Map<Artist>(from);
    }

    public ArtistName Map(ArtistNameData from)
    {
        return Map<ArtistName>(from);
    }

    public Track Map(TrackData from)
    {
        return Map<Track>(from);
    }

    public Artist Map(AlbumArtistData from)
    {
        return Map(from.Artist ?? throw new InvalidOperationException("Artist is missing."));
    }

    public Artist Map(TrackArtistData from)
    {
        return Map(from.Artist ?? throw new InvalidOperationException("Artist is missing."));
    }
}
