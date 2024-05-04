using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Samovar.Blazor.Header
{
    public partial class SmDataGridHeader<TItem>
        : SmDesignComponentBase, IAsyncDisposable
    {
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        [SmInject]
        protected IColumnService GridColumnService { get; init; }

        [SmInject]
        protected ILayoutService GridLayoutService { get; set; }

        [SmInject]
        protected ISortingService GridOrderService { get; set; }

        [SmInject]
        protected IEditingService<TItem> GridEditingService { get; set; }

        [SmInject]
        protected IGridStateService GridStateService { get; set; }

        public ElementReference GridHeaderRef;

        #region Column width per MouseMove

        protected async Task OnMouseDown(MouseEventArgs args, IDataColumnModel colMeta)
        {
            //if (DataGrid.FitColumnsToTableWidth)
            //{
            //    return;
            //}

            //await DataGrid.jsModule.InvokeVoidAsync("add_Window_MouseMove_EventListener", DataGrid.DataGridDotNetRef);
            //await DataGrid.jsModule.InvokeVoidAsync("add_Window_MouseUp_EventListener", DataGrid.DataGridDotNetRef);
            //DataGrid.ColWidthChangeManager.IsMouseDown = true;
            //DataGrid.ColWidthChangeManager.StartMouseMoveX = args.ClientX;
            //DataGrid.ColWidthChangeManager.MouseMoveCol = colMeta;
            //DataGrid.ColWidthChangeManager.OldAbsoluteVisibleWidthValue = colMeta.VisibleAbsoluteWidthValue;
            //var colEmpty = GridColumnService.Columns.Values.FirstOrDefault(cm => cm.ColumnType == GridColumnType.EmptyColumn);
            //if (colEmpty != null)
            //    DataGrid.ColWidthChangeManager.OldAbsoluteEmptyColVisibleWidthValue = colEmpty.VisibleAbsoluteWidthValue;

            ////TODO colEmpty exisitiert nicht im dynamischen Modus
            //JsInteropClasses.Start_ColumnWidthChange_Mode(DataGrid.jsModule,
            //    DataGrid.GridColWidthSum,
            //    GridColumnService.Columns.First(x => x.Value.Equals(colMeta)).Key.ToString(),
            //    DataGrid.rx.GridModelService.innerGridId,
            //    DataGrid.innerGridBodyTableId,

            //    colMeta.VisibleGridColumnCellId.ToString(),
            //    colMeta.HiddenGridColumnCellId.ToString(),
            //    colMeta.FilterGridColumnCellId.ToString(),

            //    colEmpty.VisibleGridColumnCellId.ToString(),
            //    colEmpty.HiddenGridColumnCellId.ToString(),
            //    colEmpty.FilterGridColumnCellId.ToString(),

            //    GridColumnService.Columns.First(x => x.Value.Equals(colEmpty)).Key.ToString(),
            //    args.ClientX,
            //    colMeta.VisibleAbsoluteWidthValue,
            //    DataGrid.FitColumnsToTableWidth,
            //    colEmpty.VisibleAbsoluteWidthValue);
        }

        #endregion

        #region DragDrop
        [Parameter]
        public Action<object> Drop { get; set; }

        protected async Task DropHandler(IDataColumnModel colMeta)
        {
            //if (DataGrid._ColumnDragDropService.Accepts("DropZoneGridHeader"))
            //{
            //    GridColumnService.ReplaceColumn(Guid.Parse(DataGrid._ColumnDragDropService.Data.ToString()), colMeta.Id);
            //}
            //await DataGrid.RefreshAsync();
        }

        protected void DragStartHandler(IDataColumnModel colMeta)
        {
            //DataGrid._ColumnDragDropService.StartDrag(colMeta.Id.ToString(), "DropZoneGridHeader");
        }
        #endregion

        protected Task RowInsering()
        {
            return GridEditingService.RowInsertBegin();
        }

        //Ordering handler
        internal Task ColumnCellClick(IDataColumnModel columnModel)
        {
            return GridOrderService.OnColumnClick(columnModel);
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
