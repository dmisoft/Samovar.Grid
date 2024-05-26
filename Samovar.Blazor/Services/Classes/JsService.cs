using Microsoft.JSInterop;

namespace Samovar.Blazor
{
    public class JsService(IConstantService _constantService)
                : IJsService, IAsyncDisposable
    {
        public async Task<IJSObjectReference> JsModule()
        {
            if (_module is null)
                throw new InvalidOperationException("JsModule is not initialized");
            return await _module.Value;
        }

        private Lazy<Task<IJSObjectReference>>? _module;

        public Task InitJsModule(Lazy<Task<IJSObjectReference>> module)
        {
            _module = module;
            return Task.CompletedTask;
        }

        public async Task AttachWindowResizeEvent(string DataGridId, DotNetObjectReference<ILayoutService> DataGridDotNetRef)
        {
            await (await JsModule()).InvokeVoidAsync("add_Window_OnResize_EventListener", DataGridId, DataGridDotNetRef);
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

        public async Task StartDataGridColumnWidthChangeMode(DotNetObjectReference<IColumnResizingService> colResizingService, double GridColWidthSum, string ColMetaId, string InnerGridId, string InnerGridBodyTableId, string VisibleGridColumnCellId, string HiddenGridColumnCellId, string FilterGridColumnCellId, string VisibleEmptyColumnId, string HiddenEmptyColumnId, string FilterEmptyColumnId, string EmptyColumnDictId, double StartMouseMoveX, double OldAbsoluteVisibleWidthValue, string FitColumnsToTableWidth, double OldAbsoluteEmptyColVisibleWidthValue, string? RightSideColMetaId, string? RightSideCellMetaId, double? RightSideColumnWidth, string? RightSideFilterCellId, string? RightSideHiddenCellId, string OuterGridId)
        {
            await (await JsModule()).InvokeVoidAsync("startColumnWidthChangeMode", colResizingService, GridColWidthSum, ColMetaId, InnerGridId, InnerGridBodyTableId, VisibleGridColumnCellId, HiddenGridColumnCellId, FilterGridColumnCellId, VisibleEmptyColumnId, HiddenEmptyColumnId, FilterEmptyColumnId, EmptyColumnDictId, StartMouseMoveX, OldAbsoluteVisibleWidthValue, FitColumnsToTableWidth, OldAbsoluteEmptyColVisibleWidthValue, RightSideColMetaId, RightSideCellMetaId, RightSideColumnWidth, RightSideFilterCellId, RightSideHiddenCellId, OuterGridId);
        }

        public async ValueTask<bool> IsWindowCtrlKeyDown()
        {
            return await (await JsModule()).InvokeAsync<bool>("getWindowIsCtrlKeyDown");
        }

        public async ValueTask<bool> IsWindowShiftKeyDown()
        {
            return await (await JsModule()).InvokeAsync<bool>("getWindowIsShiftKeyDown");
        }

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

        public async ValueTask<double> GetInnerGridHeight()
        {
            return await (await JsModule()).InvokeAsync<double>("getElementHeight", new[] { _constantService.InnerGridId });
        }

        public async ValueTask<double> GetElementHeightById(string elementId)
        {
            return await (await JsModule()).InvokeAsync<double>("getElementHeight", new[] { elementId });
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
            if (_module is not null && _module.IsValueCreated)
            {
                var module = await _module.Value;
                await module.DisposeAsync();
            }
        }
    }
}
