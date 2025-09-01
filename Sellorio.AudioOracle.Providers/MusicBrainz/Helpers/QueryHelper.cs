using System.Text.RegularExpressions;

namespace Sellorio.AudioOracle.Providers.MusicBrainz.Helpers;

internal partial class QueryHelper
{
    public static string EscapeValue(string input)
    {
        return MusicBrainzEscapeCharactersRegex().Replace(input, @"\$1");
    }

    [GeneratedRegex(@"(\+|\-|\&\&|\|\||\!|\(|\)|\{|\}|\[|\]|\^|\""|\~|\*|\?|\:|\\|\/)")]
    private static partial Regex MusicBrainzEscapeCharactersRegex();
}
