using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Samovar.Blazor
{
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

        //internal static ValueTask<bool> IsWindowCtrlKeyDown(IJSObjectReference jsModule)
        //{
        //    return jsModule.InvokeAsync<bool>("getWindowIsCtrlKeyDown");
        //}
        //internal static ValueTask<bool> IsWindowShiftKeyDown(IJSObjectReference jsModule)
        //{
        //    return jsModule.InvokeAsync<bool>("getWindowIsShiftKeyDown");
        //}


        internal static ValueTask<double> GetElementWidthByRef(this ElementReference elementRef, IJSObjectReference jsModule)
        {
            return jsModule.InvokeAsync<double>("getElementWidthByRef", elementRef);
        }

        internal static ValueTask SynchronizeGridHeaderScroll(this ElementReference elementRef, IJSObjectReference jsModule, string gridHeaderContainerId)
        {
            return jsModule.InvokeVoidAsync("synchronizeGridHeaderScroll", elementRef, gridHeaderContainerId);
        }

        internal static ValueTask ScrollElementVerticalByValue(IJSObjectReference jsModule, string elementId, double scrollValue)
        {
            return jsModule.InvokeVoidAsync("scrollElementVerticalByValue", elementId, scrollValue);
        }

        internal static ValueTask<string> IsPartialInView(string elementId, string innerGridId, IJSObjectReference jsModule)
        {
            return jsModule.InvokeAsync<string>("isPartialInView", elementId, innerGridId);
        }

        internal static ValueTask<double> getElementScrollTop(this ElementReference elementRef, IJSObjectReference jsModule)
        {
            return jsModule.InvokeAsync<double>("getElementScrollTopByRef", elementRef);
        }

        internal static void Start_ColumnWidthChange_Mode(IJSObjectReference jsModule, double GridColWidthSum, string ColMetaId, string InnerGridId, string InnerGridBodyTableId, string VisibleGridColumnCellId, string HiddenGridColumnCellId, string FilterGridColumnCellId, string VisibleEmptyColumnId, string HiddenEmptyColumnId, string FilterEmptyColumnId, string EmptyColumnDictId, double StartMouseMoveX, double OldAbsoluteVisibleWidthValue, bool FitColumnsToTableWidth, double OldAbsoluteEmptyColVisibleWidthValue)
        {
            jsModule.InvokeVoidAsync("startColumnWidthChangeMode", GridColWidthSum, ColMetaId, InnerGridId, InnerGridBodyTableId, VisibleGridColumnCellId, HiddenGridColumnCellId, FilterGridColumnCellId, VisibleEmptyColumnId, HiddenEmptyColumnId, FilterEmptyColumnId, EmptyColumnDictId, StartMouseMoveX, OldAbsoluteVisibleWidthValue, FitColumnsToTableWidth, OldAbsoluteEmptyColVisibleWidthValue);
        }
    }
}
