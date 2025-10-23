using MudBlazor;

namespace Sellorio.AudioOracle.Web.Client.Library.Internal;

internal static class IconHelper
{
    public static string ToSvg(this Icon icon)
    {
        return icon switch
        {
            Icon.Add => Icons.Material.Outlined.Add,
            Icon.Edit => Icons.Material.Outlined.Edit,
            Icon.Menu => Icons.Material.Outlined.Menu,
            _ => throw new System.NotSupportedException()
        };
    }
}
