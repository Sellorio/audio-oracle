using Sellorio.AudioOracle.Library.Results.Messages;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Sellorio.AudioOracle.Library.Results.Json;

internal class SerialisablePathItem
{
    [JsonPropertyName("v")]
    public JsonElement Value { get; set; }

    [JsonPropertyName("t")]
    public ResultMessagePathItemType Type { get; set; }
}
