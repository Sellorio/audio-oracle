using System;
using Microsoft.AspNetCore.Components;
using Sellorio.AudioOracle.Web.Client.Library.Providers.DisableState;

namespace Sellorio.AudioOracle.Web.Client.Library.Base;

public abstract class AoDisableComponent : ComponentBase, IDisposable
{
    private bool isDisposed;

    protected bool DisableContent => DisableStateScope.IsDisabled || Disabled;

    [CascadingParameter]
    public required IDisableStateScope DisableStateScope { private get; init; }

    [Parameter]
    public bool Disabled { private get; set; }

    protected override void OnInitialized()
    {
        DisableStateScope.IsDisabledChanged += DisabledChanged;
    }

    private void DisabledChanged()
    {
        _ = InvokeAsync(StateHasChanged);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!isDisposed)
        {
            if (disposing)
            {
                DisableStateScope.IsDisabledChanged -= DisabledChanged;
            }

            isDisposed = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
