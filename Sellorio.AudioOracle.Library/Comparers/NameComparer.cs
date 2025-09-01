using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Sellorio.AudioOracle.Library.Comparers;

public class NameComparer : IEqualityComparer<string>
{
    private NameComparer()
    {
    }

    public bool Equals(string x, string y)
    {
        return ToSearchNormalisedName(x).Equals(ToSearchNormalisedName(y));
    }

    public int GetHashCode([DisallowNull] string obj)
    {
        return ToSearchNormalisedName(obj).GetHashCode();
    }

    public static NameComparer Instance { get; } = new();

    public static string ToSearchNormalisedName(string text)
    {
        var normalized = text.Normalize(NormalizationForm.FormD);
        var filtered = normalized.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);
        var textWithoutAccents = new string(filtered.ToArray()).Normalize(NormalizationForm.FormC);

        return
            textWithoutAccents
                // remove japanese elongated vowel representation to avoid inconsistency
                .Replace("ou", "o")
                .Replace("Ou", "O")
                // YouTube: ULTRATOWER, MusicBrainz: ULTRA TOWER, Me: :(
                .Replace(" ", "")
                .ToLowerInvariant();
    }
}
