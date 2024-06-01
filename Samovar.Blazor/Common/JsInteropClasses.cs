using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Samovar.Blazor;

internal static class JsInteropClasses
{
    internal static ValueTask<double> MeasureScrollbar(IJSObjectReference jsModule)
    {
        return jsModule.InvokeAsync<double>("measureScrollbar");
    }

    internal static ValueTask<double> MeasureTableFilterHeight(IJSObjectReference jsModule, string tableClass, string tableHeaderClass, string filterToggleButtonClass, string testId)
    {
        return jsModule.InvokeAsync<double>("measureTableFilterHeight", tableClass, tableHeaderClass, filterToggleButtonClass, testId);
    }

    internal static ValueTask<double> MeasureTableRowHeight(IJSObjectReference jsModule, string tableClass, string testId)
    {
        return jsModule.InvokeAsync<double>("measureTableRowHeight", tableClass, testId);
    }

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
