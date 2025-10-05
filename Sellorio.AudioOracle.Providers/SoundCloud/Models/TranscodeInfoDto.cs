using System.Collections.Generic;

namespace Sellorio.AudioOracle.Providers.SoundCloud.Models;

internal class TranscodeInfoDto
{
    public required string Url { get; init; }
    public IList<string>? LicenseUrls { get; init; }
}
