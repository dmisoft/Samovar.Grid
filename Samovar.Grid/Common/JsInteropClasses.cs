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

    internal static ValueTask<double> GetScrollbarWidth(this ElementReference elementRef, IJSObjectReference jsModule)
    {
        return jsModule.InvokeAsync<double>("getScrollbarWidth", elementRef);
    }

    internal static ValueTask SynchronizeGridHeaderScroll(this ElementReference elementRef, IJSObjectReference jsModule, string gridHeaderContainerId)
    {
        return jsModule.InvokeVoidAsync("synchronizeGridHeaderScroll", elementRef, gridHeaderContainerId);
    }

    internal static ValueTask<double> GetElementScrollLeft(this ElementReference elementRef, IJSObjectReference jsModule)
    {
        return jsModule.InvokeAsync<double>("getElementScrollLeft", elementRef);
    }

    internal static ValueTask SetElementScrollLeft(this ElementReference elementRef, IJSObjectReference jsModule, double value)
    {
        return jsModule.InvokeVoidAsync("setElementScrollLeft", elementRef, value);
    }
}
