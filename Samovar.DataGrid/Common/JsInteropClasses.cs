using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Samovar.DataGrid
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

        //internal static ValueTask SynchronizeGridInnerBodyScroll(this ElementReference elementRef, IJSObjectReference jsModule, string gridInnerBodyId)
        //{
        //    return jsModule.InvokeVoidAsync("synchronizeGridInnerBodyScroll", elementRef, gridInnerBodyId);
        //}

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


        ///////////
        //internal static ValueTask<double> GetElementWidth(string elementId, IJSRuntime jsRuntime)
        //{
        //    return jsRuntime.InvokeAsync<double>("GridFunctions.getElementWidth", elementId);
        //}

        //internal static ValueTask<int> GetElementHeightByRef(this ElementReference elementRef, IJSRuntime jsRuntime)
        //{
        //    return jsRuntime.InvokeAsync<int>("GridFunctions.getElementHeightByRef", elementRef);
        //}
        //internal static ValueTask<bool> isVerticalScrollPresentByRef(this ElementReference elementRef, IJSRuntime jsRuntime)
        //{
        //    return jsRuntime.InvokeAsync<bool>("GridFunctions.isVerticalScrollPresentByRef", elementRef);
        //}

        //internal static ValueTask<double> getElementScrollTop(IJSRuntime jSRuntime)
        //{
        //    return jSRuntime.InvokeAsync<double>("GridFunctions.getElementScrollTop");
        //}

        //internal static ValueTask ScrollElementInView(string elementId, string innerGridId, IJSRuntime jSRuntime)
        //{
        //    return jSRuntime.InvokeVoidAsync("GridFunctions.scrollElementInView", elementId, innerGridId);
        //}

        //internal static ValueTask<double> GetElementScrollLeft(string elementId, IJSRuntime jSRuntime)
        //{
        //    return jSRuntime.InvokeAsync<double>("GridFunctions.getElementScrollLeft", elementId);
        //}

        //internal static ValueTask<double> GetElementScrollLeft(this ElementReference elementRef, IJSRuntime jsRuntime)
        //{
        //    return jsRuntime.InvokeAsync<double>("GridFunctions.getElementScrollLeftByRef", elementRef);
        //}

        //internal static ValueTask<bool> IsInView(string elementId, IJSRuntime jSRuntime)
        //{
        //    return jSRuntime.InvokeAsync<bool>("GridFunctions.isInView", elementId);
        //}

        //internal static ValueTask<string> IsInView(string elementId, string innerGridId, IJSRuntime jSRuntime)
        //{
        //    return jSRuntime.InvokeAsync<string>("GridFunctions.isInView", elementId, innerGridId);
        //}

        //internal static ValueTask Add_GridInner_OnScroll_EventHandler_ByRef<T>(this ElementReference elementRef, IJSRuntime jSRuntime, DotNetObjectReference<SamovarGrid<T>> dotNetRef)
        //{
        //    return jSRuntime.InvokeVoidAsync("GridFunctions.add_GridInner_OnScroll_EventListener_ByRef", elementRef, dotNetRef);
        //}
        //internal static ValueTask Remove_GridInner_OnScroll_EventHandler_ByRef<T>(this ElementReference elementRef, IJSRuntime jSRuntime, DotNetObjectReference<SamovarGrid<T>> dotNetRef)
        //{
        //    return jSRuntime.InvokeVoidAsync("GridFunctions.remove_GridInner_OnScroll_EventListener_ByRef", elementRef, dotNetRef);
        //}
        //internal static void StopPropagationForElement_OnKeyDown(this ElementReference elementRef, IJSObjectReference jsModule)
        //{
        //    jsModule.InvokeVoidAsync("setStopPropagationForElement_OnKeyDown", elementRef);
        //}
        //internal static void DataGridOnKeyDown_Unbind(this ElementReference elementRef, IJSObjectReference jsModule)
        //{
        //    jsModule.InvokeVoidAsync("dataGridOnKeyDown_Unbind", elementRef);
        //}
    }

    public class HtmlItemInViewCheckInfo
    {
        public bool ProbablyNotInView { get; set; }
        public float ElementTop { get; set; }
        public float ElementBottom { get; set; }
        public float ViewportTop { get; set; }
        public float ViewportBottom { get; set; }
    }
}
