using Sellorio.AudioOracle.Library.Results.Messages;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sellorio.AudioOracle.Library.Results.Json;

internal class SerialisableResultMessage
{
    [JsonPropertyName("p")]
    public IEnumerable<SerialisablePathItem> Path { get; set; }

    [JsonPropertyName("s")]
    public ResultMessageSeverity Severity { get; set; }

    [JsonPropertyName("t")]
    public string Text { get; set; }
}
