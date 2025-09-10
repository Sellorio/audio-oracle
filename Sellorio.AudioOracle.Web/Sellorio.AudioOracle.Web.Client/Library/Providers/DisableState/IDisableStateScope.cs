using System;

namespace Sellorio.AudioOracle.Web.Client.Library.Providers.DisableState;
public interface IDisableStateScope : IDisposable
{
    IDialogProvider DialogProvider { get; }
    bool IsDisabled { get; }

    event Action IsDisabledChanged;
}