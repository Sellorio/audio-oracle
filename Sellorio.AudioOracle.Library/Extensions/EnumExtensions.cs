using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Sellorio.AudioOracle.Library.Extensions;

public static partial class EnumExtensions
{
    public static string GetDisplay(this Enum value, out string? description)
    {
        var type = value.GetType();
        var member = type.GetMember(value.ToString());

        if (member.Length > 0)
        {
            var displayAttribute = member[0].GetCustomAttribute<DisplayAttribute>();

            if (displayAttribute != null && displayAttribute.Name != null)
            {
                description = displayAttribute.Description;
                return displayAttribute.Name;
            }
        }

        // Fallback: Convert PascalCase to Title Case with spaces
        string pascal = value.ToString();
        string title = PascalCaseWordBoundaryRegex().Replace(pascal, " $1");
        description = null;
        return title;
    }

    [GeneratedRegex("(\\B[A-Z])")]
    private static partial Regex PascalCaseWordBoundaryRegex();
}
