using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;

namespace Samovar.Blazor.Header
{
    public partial class SmDataGridHeaderCell
        : SmDesignComponentBase, IDisposable
    {
        [Parameter]
        public IDataColumnModel Model { get; set; }
        
        [SmInject]
        public ISortingService SortingService {  get; set; }

        [SmInject]
        public ILayoutService LayoutService { get; set; }

        [SmInject]
        public IColumnResizingService ColumnResizingService { get; set; }

        [SmInject]
        public IJsService JsService { get; set; }

        [SmInject]
        public IColumnService ColumnService { get; set; }

        [SmInject]
        public IConstantService ConstantService { get; set; }

        IDisposable _columnOrderInfoUnsubscriber;
        protected override Task OnInitializedAsync()
        {
            _columnOrderInfoUnsubscriber = SortingService.ColumnOrderInfo.Subscribe(OnOrderInfoChanged);
            return base.OnInitializedAsync();
        }

        public string SortSymbol;
        private Task OnOrderInfoChanged(DataGridColumnOrderInfo args)
        {
            string sortSymbol = "";

            if (args.Field == Model.Field.SubjectValue && args.Asc) {
                sortSymbol = "&#x2BC5;";
            }
            else if (args.Field == Model.Field.SubjectValue && !args.Asc)
            {
                sortSymbol = "&#x2BC6;";
            }
            
            SortSymbol = sortSymbol;
            
            StateHasChanged();

            return Task.CompletedTask;
        }

        internal Task ColumnCellClick()
        {
            return SortingService.OnColumnClick(Model);
        }

        protected async Task OnMouseDown(MouseEventArgs args, IDataColumnModel colMeta)
        {
            //if (DataGrid.FitColumnsToTableWidth)
            //{
            //    return;
            //}

            await JsService.AttachWindowMouseMoveEvent(LayoutService.DataGridDotNetRef);
            await JsService.AttachWindowMouseUpEvent(LayoutService.DataGridDotNetRef);

            ColumnResizingService.IsMouseDown = true;
            ColumnResizingService.StartMouseMoveX = args.ClientX;
            ColumnResizingService.MouseMoveCol = colMeta;
            ColumnResizingService.OldAbsoluteVisibleWidthValue = colMeta.VisibleAbsoluteWidthValue;

            var colEmpty = ColumnService.EmptyColumnModel;// Columns.Values.FirstOrDefault(cm => cm.ColumnType == GridColumnType.EmptyColumn);
            if (colEmpty != null)
                ColumnResizingService.OldAbsoluteEmptyColVisibleWidthValue = colEmpty.VisibleAbsoluteWidthValue;

            ////TODO colEmpty exisitiert nicht im dynamischen Modus
            await JsService.StartDataGridColumnWidthChangeMode(
                ColumnResizingService.ColumnResizingDotNetRef,
                LayoutService.GridColWidthSum,
                colMeta.Id,
                ConstantService.InnerGridId,
                ConstantService.InnerGridBodyTableId,

                colMeta.VisibleGridColumnCellId.ToString(),
                colMeta.HiddenGridColumnCellId.ToString(),
                colMeta.FilterGridColumnCellId.ToString(),

                colEmpty.VisibleGridColumnCellId.ToString(),
                colEmpty.HiddenGridColumnCellId.ToString(),
                colEmpty.FilterGridColumnCellId.ToString(),

                ColumnService.EmptyColumnModel.Id,
                args.ClientX,
                colMeta.VisibleAbsoluteWidthValue,
                LayoutService.FitColumnsToTableWidth,
                colEmpty.VisibleAbsoluteWidthValue);
        }
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

        public void Dispose()
        {
            _columnOrderInfoUnsubscriber.Dispose();
        }
    }
}
