using Microsoft.JSInterop;

namespace Samovar.Grid;

public class JsService
            : IJsService, IAsyncDisposable
{
    private Lazy<Task<IJSObjectReference>>? _module;

    public async Task<IJSObjectReference> JsModule()
    {
        if (_module is null)
            throw new InvalidOperationException("JsModule is not initialized");
        return await _module.Value;
    }

    public Task InitJsModule(IJSRuntime jsRuntime, string dataGridId, DotNetObjectReference<ILayoutService> dataGridDotNetRef, ILayoutService layoutService)
    {
        _module = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/SamovarGrid/samovar.grid.js").AsTask());
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

    public async Task StartDataGridColumnWidthChangeMode(DotNetObjectReference<IColumnResizingService> colResizingService, double GridColWidthSum, string ColMetaId, string InnerGridId, string InnerGridBodyTableId, string VisibleGridColumnCellId, string HiddenGridColumnCellId, string FilterGridColumnCellId, string VisibleEmptyColumnId, string HiddenEmptyColumnId, string FilterEmptyColumnId, string EmptyColumnDictId, double StartMouseMoveX, double OldAbsoluteVisibleWidthValue, string FitColumnsToTableWidth, double OldAbsoluteEmptyColVisibleWidthValue, string? RightSideColMetaId, string? RightSideCellMetaId, double? RightSideColumnWidth, string? RightSideFilterCellId, string? RightSideHiddenCellId, string OuterGridId, double triggerColumnMinWidth, double rightSideColumnMinWidth)
    {
        await (await JsModule()).InvokeVoidAsync("startColumnWidthChangeMode", colResizingService, GridColWidthSum, ColMetaId, InnerGridId, InnerGridBodyTableId, VisibleGridColumnCellId, HiddenGridColumnCellId, FilterGridColumnCellId, VisibleEmptyColumnId, HiddenEmptyColumnId, FilterEmptyColumnId, EmptyColumnDictId, StartMouseMoveX, OldAbsoluteVisibleWidthValue, FitColumnsToTableWidth, OldAbsoluteEmptyColVisibleWidthValue, RightSideColMetaId, RightSideCellMetaId, RightSideColumnWidth, RightSideFilterCellId, RightSideHiddenCellId, OuterGridId, triggerColumnMinWidth, rightSideColumnMinWidth);
    }

    public async ValueTask<bool> IsWindowCtrlKeyDown()
    {
        return await (await JsModule()).InvokeAsync<bool>("getWindowIsCtrlKeyDown");
    }

    public async ValueTask<bool> IsWindowShiftKeyDown()
    {
        return await (await JsModule()).InvokeAsync<bool>("getWindowIsShiftKeyDown");
    }

    public async Task DownloadFileAsync(string fileName, string contentType, byte[] data)
    {
        await (await JsModule()).InvokeVoidAsync("downloadFile", fileName, contentType, data);
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
