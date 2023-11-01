using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public class JsService
        : IJsService, IAsyncDisposable
    {
        private IConstantService _constantService;
        //[Inject]
        //public IJSRuntime JsRuntime { get; set; }

        public JsService(IConstantService constantService)
        {
            _constantService = constantService;
            //_module = new(() => JsRuntime.InvokeAsync<IJSObjectReference>(
            //       "import", "./_content/Samovar.Blazor/samovar.blazor.js").AsTask());
        }

        Task<IJSObjectReference> jsmod => _module.Value;
        public async Task<IJSObjectReference> JsModule()
        {
            return await _module.Value;
        }
        
        private Lazy<Task<IJSObjectReference>> _module;

        public Task InitJsModule2(Lazy<Task<IJSObjectReference>> module)
        {
            _module = module;
            return Task.CompletedTask;
        }

        public async Task AttachWindowResizeEvent(string DataGridId, DotNetObjectReference<ILayoutService> DataGridDotNetRef)
        {
            await (await JsModule()).InvokeVoidAsync("add_Window_OnResize_EventListener", DataGridId, DataGridDotNetRef);
        }

        public async Task AttachOnScrollollingEvent(string InnerDataGridId, DotNetObjectReference<IVirtualScrollingNavigationStrategy> InnerDataGridDotNetRef)
        {
            await (await JsModule()).InvokeVoidAsync("remove_GridInner_OnScroll_EventListener", InnerDataGridId, InnerDataGridDotNetRef);
            await (await JsModule()).InvokeVoidAsync("add_GridInner_OnScroll_EventListener", InnerDataGridId, InnerDataGridDotNetRef);
        }

        public async Task DetachOnScrollollingEvent(string InnerDataGridId, DotNetObjectReference<IVirtualScrollingNavigationStrategy> InnerDataGridDotNetRef)
        {
            await (await JsModule()).InvokeVoidAsync("remove_GridInner_OnScroll_EventListener", InnerDataGridId, InnerDataGridDotNetRef);
        }

        public async Task AttachWindowMouseMoveEvent(DotNetObjectReference<ILayoutService> DataGridDotNetRef)
        {
            await (await JsModule()).InvokeVoidAsync("add_Window_MouseMove_EventListener", DataGridDotNetRef);
        }

        public async Task AttachWindowMouseUpEvent(DotNetObjectReference<ILayoutService> DataGridDotNetRef)
        {
            await (await JsModule()).InvokeVoidAsync("add_Window_MouseUp_EventListener", DataGridDotNetRef);
        }

        public async Task DetachWindowMouseMoveEvent()
        {
            await (await JsModule()).InvokeVoidAsync("remove_Window_MouseMove_EventListener");
        }

        public async Task DetachWindowMouseUpEvent()
        {
            await (await JsModule()).InvokeVoidAsync("remove_Window_MouseUp_EventListener");
        }

        public async Task StartDataGridColumnWidthChangeMode(DotNetObjectReference<IColumnResizingService> colResizingService, double GridColWidthSum, string ColMetaId, string InnerGridId, string InnerGridBodyTableId, string VisibleGridColumnCellId, string HiddenGridColumnCellId, string FilterGridColumnCellId, string VisibleEmptyColumnId, string HiddenEmptyColumnId, string FilterEmptyColumnId, string EmptyColumnDictId, double StartMouseMoveX, double OldAbsoluteVisibleWidthValue, bool FitColumnsToTableWidth, double OldAbsoluteEmptyColVisibleWidthValue)
        {
            await (await JsModule()).InvokeVoidAsync("startColumnWidthChangeMode", colResizingService, GridColWidthSum, ColMetaId, InnerGridId, InnerGridBodyTableId, VisibleGridColumnCellId, HiddenGridColumnCellId, FilterGridColumnCellId, VisibleEmptyColumnId, HiddenEmptyColumnId, FilterEmptyColumnId, EmptyColumnDictId, StartMouseMoveX, OldAbsoluteVisibleWidthValue, FitColumnsToTableWidth, OldAbsoluteEmptyColVisibleWidthValue);
        }

        public async ValueTask<bool> IsWindowCtrlKeyDown()
        {
            return await (await JsModule()).InvokeAsync<bool>("getWindowIsCtrlKeyDown");
        }

        public async ValueTask<bool> IsWindowShiftKeyDown()
        {
            return await (await JsModule()).InvokeAsync<bool>("getWindowIsShiftKeyDown");
        }

        //public Task InitJsModule(IJSObjectReference jsModule)
        //{
        //    //JsModule = jsModule;

        //    return Task.CompletedTask;
        //    //JsModule = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Samovar.Blazor/samovar.blazor.js");
        //}

        public async ValueTask<double> MeasureScrollbar()
        {
            return await JsInteropClasses.MeasureScrollbar(await JsModule());
        }
        public async ValueTask<double> MeasureTableFilterHeight(string tableClass, string tableHeaderClass, string filterToggleButtonClass)
        {
            return await JsInteropClasses.MeasureTableFilterHeight(await JsModule(), tableClass, tableHeaderClass, filterToggleButtonClass, $"measure{Guid.NewGuid().ToString().Replace("-", "")}");
        }
        public async ValueTask<double> MeasureTableRowHeight(string tableClass)
        {
            return await JsInteropClasses.MeasureTableRowHeight(await JsModule(), tableClass, $"measure{Guid.NewGuid().ToString().Replace("-", "")}");
        }

        public async ValueTask<double> GetInnerGridHeight() { 
            return await (await JsModule()).InvokeAsync<double>("getElementHeight", new[] { _constantService.InnerGridId });
        }

        public async ValueTask ScrollInnerGridToTop()
        {
            await ScrollElementVerticalByValue(_constantService.InnerGridId, 0);
        }

        private async ValueTask ScrollElementVerticalByValue(string elementId, double scrollValue)
        {
            await (await JsModule()).InvokeVoidAsync("scrollElementVerticalByValue", elementId, scrollValue);
        }

        public async ValueTask DisposeAsync()
        {
            if (_module.IsValueCreated)
            {
                var module = await _module.Value;
                await module.DisposeAsync();
            }
        }
    }
}
