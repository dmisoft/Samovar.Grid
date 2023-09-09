using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Samovar.DataGrid
{
    public partial class GridRow_Std<TItem>
        : IDisposable
    {
        [CascadingParameter(Name = "datagrid-container")]
        protected SamovarGrid<TItem> DataGrid { get; set; }

        [CascadingParameter(Name = "datagrid-gridcolumnservice")]
        protected GridColumnService GridColumnService { get; set; }

        [CascadingParameter(Name = "datagrid-row")]
        protected GridRow<TItem> GridRow { get; set; }

        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        int dataHashCode = 0;
        [Parameter]
        public GridRowModel<TItem> RowModel
        {
            get { return _rowModel; }
            set
            {
                _rowModel = value;
                if (dataHashCode != value.GetHashCode())
                {
                    _rowModel.NotifyAfterLoadData -= Value_NotifyAfterLoadData;
                    value.NotifyAfterLoadData -= Value_NotifyAfterLoadData;
                    value.NotifyAfterLoadData += Value_NotifyAfterLoadData;
                    dataHashCode = value.GetHashCode();
                }
            }
        }

        protected override void OnInitialized()
        {
            DataGrid.NotifierService.NotifyAfterSort += NotifierService_ResetDetailPanel;
            DataGrid.NotifierService.NotifyAfterFilter += NotifierService_ResetDetailPanel;
            DataGrid.NotifierService.NotifyAfterPagingChange += NotifierService_ResetDetailPanel;
            //DataGrid.NotifierService.NotifyAfterEditingEnd += NotifierService_NotifyAfterEditingEnd;
        }

        //private async Task NotifierService_NotifyAfterEditingEnd()
        //{
        //    await InvokeAsync(StateHasChanged);
        //}

        private Task NotifierService_ResetDetailPanel()
        {
            GridRow.ShowDetailPanel = false;
            return Task.CompletedTask;
        }

        private async Task Value_NotifyAfterLoadData()
        {
            await InvokeAsync(StateHasChanged);
        }

        private GridRowModel<TItem> _rowModel;

        //Row editing
        protected async Task RowEditBegin(GridRowModel<TItem> rowMainModel)
        {
            if(DataGrid.GridEditingService.GridState != GridState.Idle)
                await DataGrid.GridEditingService.RowEditCancel();
            
            await DataGrid.GridEditingService.RowEditBegin(rowMainModel, DataGrid.EditFormTemplate == null);
            await DataGrid.Repaint();
        }

        protected async Task RowEditCommit(GridRowModel<TItem> rowMainModel)
        {
            await DataGrid.GridEditingService.RowEditCommit();
            StateHasChanged();
        }

        protected async Task RowEditCancel(GridRowModel<TItem> rowMainModel)
        {
            await DataGrid.GridEditingService.RowEditCancel();
            StateHasChanged();
        }

        ////Row deleting
        protected async Task RowDeleteBegin(GridRowModel<TItem> rowMainModel)
        {
            await DataGrid.GridEditingService.RowDeleteBegin(rowMainModel);
        }

        protected async Task MouseDownOnResizeColumnGrip(MouseEventArgs args, ColumnMetadata colMeta)
        {
            if (DataGrid.FitColumnsToTableWidth)
            {
                return;
            }
            await DataGrid.jsModule.InvokeVoidAsync("add_Window_MouseMove_EventListener", DataGrid.DataGridDotNetRef);
            await DataGrid.jsModule.InvokeVoidAsync("add_Window_MouseUp_EventListener", DataGrid.DataGridDotNetRef);

            DataGrid.ColWidthChangeManager.IsMouseDown = true;
            DataGrid.ColWidthChangeManager.StartMouseMoveX = args.ClientX;
            DataGrid.ColWidthChangeManager.MouseMoveCol = colMeta;

            DataGrid.ColWidthChangeManager.OldAbsoluteVisibleWidthValue = colMeta.VisibleAbsoluteWidthValue;

            var colEmpty = GridColumnService.Columns.Values.FirstOrDefault(cm => cm.ColumnType == GridColumnType.EmptyColumn);
            if (colEmpty != null)
                DataGrid.ColWidthChangeManager.OldAbsoluteEmptyColVisibleWidthValue = colEmpty.VisibleAbsoluteWidthValue;

            JsInteropClasses.Start_ColumnWidthChange_Mode(DataGrid.jsModule,
                DataGrid.GridColWidthSum,
                GridColumnService.Columns.First(x => x.Value.Equals(colMeta)).Key.ToString(),
                DataGrid.rx.GridModelService.innerGridId,
                DataGrid.innerGridBodyTableId,

                colMeta.VisibleGridColumnCellId.ToString(),
                colMeta.HiddenGridColumnCellId.ToString(),
                colMeta.FilterGridColumnCellId.ToString(),

                colEmpty.VisibleGridColumnCellId.ToString(),
                colEmpty.HiddenGridColumnCellId.ToString(),
                colEmpty.FilterGridColumnCellId.ToString(),

                GridColumnService.Columns.First(x => x.Value.Equals(colEmpty)).Key.ToString(),
                args.ClientX,
                colMeta.VisibleAbsoluteWidthValue,
                DataGrid.FitColumnsToTableWidth,
                colEmpty.VisibleAbsoluteWidthValue);
        }

        public void Dispose()
        {
            DataGrid.NotifierService.NotifyAfterSort -= NotifierService_ResetDetailPanel;
            DataGrid.NotifierService.NotifyAfterFilter -= NotifierService_ResetDetailPanel;
            DataGrid.NotifierService.NotifyAfterPagingChange -= NotifierService_ResetDetailPanel;
            //DataGrid.NotifierService.NotifyAfterEditingEnd -= NotifierService_NotifyAfterEditingEnd;

            _rowModel.NotifyAfterLoadData -= Value_NotifyAfterLoadData;
            GC.Collect();
        }
    }
}
