using System;
using System.Collections.Generic;

namespace Sellorio.AudioOracle.Providers.MusicBrainz.Dtos;

internal class RecordingsSearchResultDto
{
    public required DateTimeOffset Created { get; init; }
    public required int Count { get; init; }
    public required int Offset { get; init; }
    public required IList<RecordingDto> Recordings { get; init; }
}
