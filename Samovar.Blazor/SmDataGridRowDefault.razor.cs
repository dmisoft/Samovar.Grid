using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public partial class SmDataGridRowDefault<T>
        : SmDesignComponentBase, IDisposable//, IModelContainer<T>
    {
        [CascadingParameter(Name = "datagrid-row")]
        protected SmDataGridRow<T> GridRow { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [SmInject]
        public IColumnService ColumnService { get; set; }

        [SmInject]
        public ILayoutService LayoutService { get; set; }

        [SmInject]
        public IGridStateService GridStateService { get; set; }

        [SmInject]
        public IEditingService<T> EditingService { get; set; }

        [SmInject]
        public ITemplateService<T> TemplateService { get; set; }

        [SmInject]
        public IGridSelectionService<T> GridSelectionService { get; set; }

        [SmInject]
        protected IRowDetailService<T> RowDetailService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [Parameter]
        public SmDataGridRowModel<T> RowModel
        {
            get { return _rowModel; }
            set
            {
                _rowModel = value;

                _rowModel.RowSelected = false;

                switch (GridSelectionService.SelectionMode.Value)
                {
                    case GridSelectionMode.None:
                        break;
                    case GridSelectionMode.SingleSelectedDataRow:
                        _rowModel.RowSelected = GridSelectionService.SingleSelectedDataRow.Value != null && GridSelectionService.SingleSelectedDataRow.Value.Equals(_rowModel.DataItem);
                        break;
                    case GridSelectionMode.MultipleSelectedDataRows:
                        _rowModel.RowSelected = GridSelectionService.MultipleSelectedDataRows.Value != null && GridSelectionService.MultipleSelectedDataRows.Value.Any(a => a.Equals(_rowModel.DataItem));
                        break;
                    default:
                        break;
                }
            }
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
        }

        public override Task SetParametersAsync(ParameterView parameters)
        {
            return base.SetParametersAsync(parameters);
        }

        private SmDataGridRowModel<T> _rowModel;

        //Row editing
        protected async Task RowEditBegin(SmDataGridRowModel<T> rowMainModel)
        {
            if (GridStateService.DataSourceState.Value != DataSourceState.Idle)
                await EditingService.RowEditCancel();

            await EditingService.RowEditBegin(rowMainModel);
        }

        protected async Task RowEditCommit(SmDataGridRowModel<T> rowMainModel)
        {
            //await DataGrid.GridEditingService.RowEditCommit();
            //StateHasChanged();
        }

        protected async Task RowEditCancel(SmDataGridRowModel<T> rowMainModel)
        {
            //await DataGrid.GridEditingService.RowEditCancel();
            //StateHasChanged();
        }

        ////Row deleting
        protected async Task RowDeleteBegin(SmDataGridRowModel<T> rowMainModel)
        {
            await EditingService.RowDeleteBegin(rowMainModel);
        }

        protected async Task MouseDownOnResizeColumnGrip(MouseEventArgs args, IDataColumnModel colMeta)
        {

            //if (LayoutService.FitColumnsToTableWidth)
            //{
            //    return;
            //}
            //await JsService.JsModule.InvokeVoidAsync("add_Window_MouseMove_EventListener", LayoutService.DataGridDotNetRef);//TODO DataGridDotNetRef braucht man nicht
            //await JsService.JsModule.InvokeVoidAsync("add_Window_MouseUp_EventListener", LayoutService.DataGridDotNetRef);

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

        public void Dispose()
        {
            SingleSelectedDataRowsSubscription.Dispose();
            
            MultipleSelectedDataRowsSubscription.Dispose();

            EditingService.RowEditingEnded -= EditingService_RowEditingEnded;

            GC.Collect();
        }
        internal async Task RowSelectedIntern(GridRowEventArgs args, SmDataGridRowModel<T> selectedModel)
        {
            await GridSelectionService.OnRowSelected(selectedModel.DataItem);
            //return Task.CompletedTask;
        }
        internal void FireRowDoubleClick(GridRowEventArgs args, SmDataGridRowModel<T> mainModel)
        {
            //if (RowDoubleClickAsync != null)
            //    RowDoubleClickAsync.Invoke(mainModel.dataItem);
            //else
            //    RowDoubleClick?.Invoke(mainModel.dataItem);
        }

        IDisposable SingleSelectedDataRowsSubscription;

        IDisposable MultipleSelectedDataRowsSubscription;

        protected override Task OnInitializedAsync()
        {
            SingleSelectedDataRowsSubscription = GridSelectionService.SingleSelectedDataRow.Subscribe(SingleSelectedDataRowsChanged);

            MultipleSelectedDataRowsSubscription = GridSelectionService.MultipleSelectedDataRows.Subscribe(MultipleSelectedDataRowsChanged);

            EditingService.RowEditingEnded += EditingService_RowEditingEnded;
            
            return base.OnInitializedAsync();
        }

        private Task EditingService_RowEditingEnded()
        {
            StateHasChanged();
            return Task.CompletedTask;
        }

        private void MultipleSelectedDataRowsChanged(IEnumerable<T> arg)
        {
            _rowModel.RowSelected = arg != null && arg.Any(a => a.Equals(_rowModel.DataItem));

            StateHasChanged();

            //return Task.CompletedTask;
        }

        private void SingleSelectedDataRowsChanged(T arg)
        {
            _rowModel.RowSelected = arg != null && arg.Equals(_rowModel.DataItem);

            StateHasChanged();
            
            //return Task.CompletedTask;
        }
    }
}
