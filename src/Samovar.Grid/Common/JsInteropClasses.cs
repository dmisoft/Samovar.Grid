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

    internal static ValueTask<double> GetElementScrollLeft(this ElementReference elementRef, IJSObjectReference jsModule)
    {
        return jsModule.InvokeAsync<double>("getElementScrollLeft", elementRef);
    }

    internal static ValueTask SetElementScrollLeft(this ElementReference elementRef, IJSObjectReference jsModule, double value)
    {
        return jsModule.InvokeVoidAsync("setElementScrollLeft", elementRef, value);
    }

    internal static ValueTask InitCustomScrollbars(this ElementReference contentRef, IJSObjectReference jsModule,
        ElementReference vTrack, ElementReference hTrack, ElementReference vThumb, ElementReference hThumb)
    {
        return jsModule.InvokeVoidAsync("initCustomScrollbars", contentRef, vTrack, hTrack, vThumb, hThumb);
    }

    internal static ValueTask DisposeCustomScrollbars(this ElementReference contentRef, IJSObjectReference jsModule)
    {
        return jsModule.InvokeVoidAsync("disposeCustomScrollbars", contentRef);
    }
}
