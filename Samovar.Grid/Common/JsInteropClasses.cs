using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Samovar.Grid;

internal static class JsInteropClasses
{
    internal static ValueTask<bool> IsWindowCtrlKeyDown(IJSObjectReference jsModule)
    {
        return jsModule.InvokeAsync<bool>("getWindowIsCtrlKeyDown");
    }

    internal static ValueTask<bool> IsWindowShiftKeyDown(IJSObjectReference jsModule)
    {
        return jsModule.InvokeAsync<bool>("getWindowIsShiftKeyDown");
    }

    internal static ValueTask<double> GetElementWidthByRef(this ElementReference elementRef, IJSObjectReference jsModule)
    {
        return jsModule.InvokeAsync<double>("getElementWidthByRef", elementRef);
    }

    internal static ValueTask SynchronizeGridHeaderScroll(this ElementReference elementRef, IJSObjectReference jsModule, string gridHeaderContainerId)
    {
        return jsModule.InvokeVoidAsync("synchronizeGridHeaderScroll", elementRef, gridHeaderContainerId);
    }
}
