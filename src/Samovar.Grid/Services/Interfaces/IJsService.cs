using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Samovar.Grid;

public record ElementBoundingRect(double Top, double Left, double Width, double Height);

public interface IJsService
{
    Task<IJSObjectReference> JsModule();

    Task InitJsModule(IJSRuntime jsRuntime, string dataGridId, DotNetObjectReference<ILayoutService> dataGridDotNetRef, ILayoutService layoutService);

    Task AttachWindowResizeEvent(string DataGridId, DotNetObjectReference<ILayoutService> DataGridDotNetRef);

    Task AttachWindowMouseMoveEvent(DotNetObjectReference<ILayoutService> DataGridDotNetRef);

    Task AttachWindowMouseUpEvent(DotNetObjectReference<ILayoutService> DataGridDotNetRef);

    Task DetachWindowMouseMoveEvent();

    Task DetachWindowMouseUpEvent();

    ValueTask<bool> IsWindowCtrlKeyDown();

    ValueTask<bool> IsWindowShiftKeyDown();

    Task StartDataGridColumnWidthChangeMode(
        DotNetObjectReference<IColumnResizingService> colResizingService,
        double GridColWidthSum,
        string ColMetaId,
        string InnerGridId,
        string InnerGridBodyTableId,
        string VisibleGridColumnCellId,
        string HiddenGridColumnCellId,
        string FilterGridColumnCellId,
        string VisibleEmptyColumnId,
        string HiddenEmptyColumnId,
        string FilterEmptyColumnId,
        string EmptyColumnDictId,
        double StartMouseMoveX,
        double OldAbsoluteVisibleWidthValue,
        string FitColumnsToTableWidth,
        double OldAbsoluteEmptyColVisibleWidthValue,
        string? RightSideColMetaId,
        string? RightSideCellMetaId,
        double? RightSideColumnWidth,
        string? RightSideFilterCellId,
        string? RightSideHiddenCellId,
        string OuterGridId,
        double triggerColumnMinWidth,
        double rightSideColumnMinWidth);

    Task DownloadFileAsync(string fileName, string contentType, byte[] data);

    ValueTask<ElementBoundingRect> GetElementBoundingRect(ElementReference element);

    Task AddFilterMenuDismissHandlers(ElementReference element, object dotNetRef);

    Task RemoveFilterMenuDismissHandlers();
}
