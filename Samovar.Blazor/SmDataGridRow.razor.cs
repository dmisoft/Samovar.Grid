using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public partial class SmDataGridRow<TItem>
        : SmDesignComponentBase, IDisposable
    {
        [SmInject]
        protected IColumnService GridColumnService { get; set; }
        
        [SmInject]
        protected IGridStateService GridStateService { get; set; }

        [SmInject]
        protected ITemplateService<TItem> TemplateService { get; set; }

        [SmInject]
        protected IEditingService<TItem> EditingService { get; set; }

        [SmInject]
        protected IGridStateService StateService { get; set; }

        [SmInject]
        protected IRowDetailService<TItem> RowDetailService { get; set; }

        [SmInject]
        public IComponentBuilderService ComponentBuilderService { get; set; }

        private SmDataGridRowModel<TItem> _rowModel;

        [Parameter]
        public EventCallback<SmDataGridRowModel<TItem>> RowModelChanged { get; set; }

        //int dataHashCode = 0;
        [Parameter]
        public SmDataGridRowModel<TItem> RowModel
        {
            get { return _rowModel; }
            set
            {
                _rowModel = value;
                //if (dataHashCode != value.GetHashCode())
                //{
                //    dataHashCode = value.GetHashCode();
                //}
            }
        }

        protected async Task MouseDownOnResizeColumnGrip(MouseEventArgs args, DataColumnModel colMeta)
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

		//internal bool ShowDetailPanel { get; set; }
		internal async Task DetailExpanderClick()
		{
			RowModel.RowDetailExpanded = !RowModel.RowDetailExpanded;
            await RowDetailService.ExpandOrCloseRowDetails(RowModel.DataItem);
        }

		public void Dispose()
        {
            //DataGrid.NotifierService.NotifyAfterSort -= NotifierService_ResetDetailPanel;
            //DataGrid.NotifierService.NotifyAfterFilter -= NotifierService_ResetDetailPanel;
            //DataGrid.NotifierService.NotifyAfterPagingChange -= NotifierService_ResetDetailPanel;
            //DataGrid.NotifierService.NotifyAfterEditingEnd -= NotifierService_NotifyAfterEditingEnd;

            //_rowModel.NotifyAfterLoadData -= Value_NotifyAfterLoadData;
            GC.Collect();
        }
    }
}
