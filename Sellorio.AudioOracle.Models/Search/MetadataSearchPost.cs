using System.Collections.Generic;

namespace Sellorio.AudioOracle.Models.Search;

public class MetadataSearchPost
{
    public required string SearchText { get; init; }
    public required IList<string>? IncludedProviders { get; init; }
}
