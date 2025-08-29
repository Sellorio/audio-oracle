using System.Text.Json.Serialization;
using System.Text.Json;
using System.Collections.Generic;

namespace Sellorio.AudioOracle.Library.Results.Json;

internal class SerialisableResult
{
    [JsonPropertyName("m")]
    public IEnumerable<SerialisableResultMessage> Messages { get; set; }

    [JsonPropertyName("v")]
    public JsonElement Value { get; set; }
}