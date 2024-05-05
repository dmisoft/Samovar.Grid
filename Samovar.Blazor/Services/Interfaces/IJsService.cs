using Microsoft.JSInterop;

namespace Samovar.Blazor
{
    public interface IJsService
    {
        Task<IJSObjectReference> JsModule();

        Task InitJsModule(Lazy<Task<IJSObjectReference>> module);

        Task AttachWindowResizeEvent(string DataGridId, DotNetObjectReference<ILayoutService> DataGridDotNetRef);

        Task AttachOnScrollollingEvent(string InnerDataGridId, DotNetObjectReference<IVirtualScrollingNavigationStrategy> InnerDataGridDotNetRef);

        Task DetachOnScrollollingEvent(string InnerDataGridId, DotNetObjectReference<IVirtualScrollingNavigationStrategy> InnerDataGridDotNetRef);

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
            bool FitColumnsToTableWidth, 
            double OldAbsoluteEmptyColVisibleWidthValue);

        ValueTask<double> MeasureScrollbar();
        
        ValueTask<double> MeasureTableFilterHeight(string tableClass, string tableHeaderClass, string filterToggleButtonClass);
        
        ValueTask<double> MeasureTableRowHeight(string tableClass);
        
        ValueTask<double> GetInnerGridHeight();

        ValueTask<double> GetElementHeightById(string elementId);
        
        ValueTask ScrollInnerGridToTop();
    }
}
