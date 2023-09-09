//using System;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Components;
//using Microsoft.JSInterop;

//namespace Samovar.DataGrid
//{
//    public static class JaInteropClasses {
//        public static async ValueTask<double> GetElementWidthByRef(this ElementReference elementRef, IJSObjectReference jsModule)
//        {
//            return await jsModule.InvokeAsync<double>("getElementWidthByRef", elementRef);
//        }

//        public static async ValueTask SynchronizeGridHeaderScroll(this ElementReference elementRef, IJSObjectReference jsModule, string gridHeaderContainerId)
//        {
//            await jsModule.InvokeVoidAsync("synchronizeGridHeaderScroll", elementRef, gridHeaderContainerId);
//        }

//        public static async ValueTask SynchronizeGridInnerBodyScroll(this ElementReference elementRef, IJSObjectReference jsModule, string gridInnerBodyId)
//        {
//            await jsModule.InvokeVoidAsync("synchronizeGridInnerBodyScroll", elementRef, gridInnerBodyId);
//        }

//        public static async ValueTask<double> getElementScrollTop(this ElementReference elementRef, IJSObjectReference jsModule)
//        {
//            return await jsModule.InvokeAsync<double>("getElementScrollTopByRef", elementRef);
//        }
//    }
//    public class DataGridJsInterop<T>
//    : IAsyncDisposable
//    {
//        private readonly Lazy<Task<IJSObjectReference>> moduleTask;
//        private SamovarGrid<T> dataGridInstance;
//        public DataGridJsInterop(IJSRuntime jsRuntime)
//        {
//            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
//               "import", "./_content/SamovarGrid/samovar.grid.js").AsTask());
//            //jsModule = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/SamovarGrid/samovar.grid.js");
//        }
//        public async ValueTask Init(SamovarGrid<T> instance)
//        {
//            dataGridInstance = instance;
//            var module = await moduleTask.Value;
//            await module.InvokeVoidAsync("init", DotNetObjectReference.Create(this));
//        }
//        public async ValueTask<double> MeasureScrollbar(IJSObjectReference jsModule)
//        {
//            return await jsModule.InvokeAsync<double>("measureScrollbar");
//        }
//        public async ValueTask<double> MeasureTableFilterHeight(IJSObjectReference jsModule, string tableClass, string tableHeaderClass, string filterToggleButtonClass, string testId)
//        {
//            return await jsModule.InvokeAsync<double>("measureTableFilterHeight", tableClass, tableHeaderClass, filterToggleButtonClass, testId);
//        }
//        public async ValueTask<double> MeasureTableRowHeight(IJSObjectReference jsModule, string tableClass, string testId)
//        {
//            return await jsModule.InvokeAsync<double>("measureTableRowHeight", tableClass, testId);
//        }

//        public async ValueTask<bool> IsWindowCtrlKeyDown()
//        {
//            var module = await moduleTask.Value;
//            return await module.InvokeAsync<bool>("getWindowIsCtrlKeyDown");
//        }
//        public async ValueTask<bool> IsWindowShiftKeyDown()
//        {
//            var module = await moduleTask.Value;
//            return await module.InvokeAsync<bool>("getWindowIsShiftKeyDown");
//        }



//        public async ValueTask<string> IsPartialInView(string elementId, string innerGridId)
//        {
//            var module = await moduleTask.Value;
//            return await module.InvokeAsync<string>("isPartialInView", elementId, innerGridId);
//        }



//        public async ValueTask Start_ColumnWidthChange_Mode(IJSObjectReference jsModule, double GridColWidthSum, string ColMetaId, string InnerGridId, string InnerGridBodyTableId, string VisibleGridColumnCellId, string HiddenGridColumnCellId, string FilterGridColumnCellId, string VisibleEmptyColumnId, string HiddenEmptyColumnId, string FilterEmptyColumnId, string EmptyColumnDictId, double StartMouseMoveX, double OldAbsoluteVisibleWidthValue, bool FitColumnsToTableWidth, double OldAbsoluteEmptyColVisibleWidthValue)
//        {
//            await jsModule.InvokeVoidAsync("startColumnWidthChangeMode", GridColWidthSum, ColMetaId, InnerGridId, InnerGridBodyTableId, VisibleGridColumnCellId, HiddenGridColumnCellId, FilterGridColumnCellId, VisibleEmptyColumnId, HiddenEmptyColumnId, FilterEmptyColumnId, EmptyColumnDictId, StartMouseMoveX, OldAbsoluteVisibleWidthValue, FitColumnsToTableWidth, OldAbsoluteEmptyColVisibleWidthValue);
//        }

//        public async ValueTask DisposeAsync()
//        {
//            throw new NotImplementedException();
//        }


//        //        ///////////
//        //        //public static ValueTask<double> GetElementWidth(string elementId, IJSRuntime jsRuntime)
//        //        //{
//        //        //    return jsRuntime.InvokeAsync<double>("GridFunctions.getElementWidth", elementId);
//        //        //}

//        //        //public static ValueTask<int> GetElementHeightByRef(this ElementReference elementRef, IJSRuntime jsRuntime)
//        //        //{
//        //        //    return jsRuntime.InvokeAsync<int>("GridFunctions.getElementHeightByRef", elementRef);
//        //        //}
//        //        //public static ValueTask<bool> isVerticalScrollPresentByRef(this ElementReference elementRef, IJSRuntime jsRuntime)
//        //        //{
//        //        //    return jsRuntime.InvokeAsync<bool>("GridFunctions.isVerticalScrollPresentByRef", elementRef);
//        //        //}

//        //        //public static ValueTask<double> getElementScrollTop(IJSRuntime jSRuntime)
//        //        //{
//        //        //    return jSRuntime.InvokeAsync<double>("GridFunctions.getElementScrollTop");
//        //        //}

//        //        //public static ValueTask ScrollElementInView(string elementId, string innerGridId, IJSRuntime jSRuntime)
//        //        //{
//        //        //    return jSRuntime.InvokeVoidAsync("GridFunctions.scrollElementInView", elementId, innerGridId);
//        //        //}

//        //        //public static ValueTask<double> GetElementScrollLeft(string elementId, IJSRuntime jSRuntime)
//        //        //{
//        //        //    return jSRuntime.InvokeAsync<double>("GridFunctions.getElementScrollLeft", elementId);
//        //        //}

//        //        //public static ValueTask<double> GetElementScrollLeft(this ElementReference elementRef, IJSRuntime jsRuntime)
//        //        //{
//        //        //    return jsRuntime.InvokeAsync<double>("GridFunctions.getElementScrollLeftByRef", elementRef);
//        //        //}

//        //        //public static ValueTask<bool> IsInView(string elementId, IJSRuntime jSRuntime)
//        //        //{
//        //        //    return jSRuntime.InvokeAsync<bool>("GridFunctions.isInView", elementId);
//        //        //}

//        //        //public static ValueTask<string> IsInView(string elementId, string innerGridId, IJSRuntime jSRuntime)
//        //        //{
//        //        //    return jSRuntime.InvokeAsync<string>("GridFunctions.isInView", elementId, innerGridId);
//        //        //}

//        //        //public static ValueTask Add_GridInner_OnScroll_EventHandler_ByRef<T>(this ElementReference elementRef, IJSRuntime jSRuntime, DotNetObjectReference<SamovarGrid<T>> dotNetRef)
//        //        //{
//        //        //    return jSRuntime.InvokeVoidAsync("GridFunctions.add_GridInner_OnScroll_EventListener_ByRef", elementRef, dotNetRef);
//        //        //}
//        //        //public static ValueTask Remove_GridInner_OnScroll_EventHandler_ByRef<T>(this ElementReference elementRef, IJSRuntime jSRuntime, DotNetObjectReference<SamovarGrid<T>> dotNetRef)
//        //        //{
//        //        //    return jSRuntime.InvokeVoidAsync("GridFunctions.remove_GridInner_OnScroll_EventListener_ByRef", elementRef, dotNetRef);
//        //        //}
//        //        //public static void StopPropagationForElement_OnKeyDown(this ElementReference elementRef, IJSObjectReference jsModule)
//        //        //{
//        //        //    jsModule.InvokeVoidAsync("setStopPropagationForElement_OnKeyDown", elementRef);
//        //        //}
//        //        //public static void DataGridOnKeyDown_Unbind(this ElementReference elementRef, IJSObjectReference jsModule)
//        //        //{
//        //        //    jsModule.InvokeVoidAsync("dataGridOnKeyDown_Unbind", elementRef);
//        //        //}
//    }

//    public class HtmlItemInViewCheckInfo
//    {
//        public bool ProbablyNotInView { get; set; }
//        public float ElementTop { get; set; }
//        public float ElementBottom { get; set; }
//        public float ViewportTop { get; set; }
//        public float ViewportBottom { get; set; }
//    }
//}
