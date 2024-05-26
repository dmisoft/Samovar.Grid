using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Reactive.Linq;

namespace Samovar.Blazor.Header
{
    public partial class SmDataGridHeaderCell
        : SmDesignComponentBase, IAsyncDisposable
    {
        [Parameter]
        public required IDataColumnModel Model { get; set; }

        [SmInject]
        public required ISortingService SortingService { get; set; }

        [SmInject]
        public required ILayoutService LayoutService { get; set; }

        [SmInject]
        public required IColumnResizingService ColumnResizingService { get; set; }

        [SmInject]
        public required IJsService JsService { get; set; }

        [SmInject]
        public required IColumnService ColumnService { get; set; }

        [SmInject]
        public required IConstantService ConstantService { get; set; }

        IDisposable? _columnOrderInfoUnsubscriber = null;
        protected override Task OnInitializedAsync()
        {
            _columnOrderInfoUnsubscriber = SortingService.ColumnOrderInfo.Subscribe(OnOrderInfoChanged);
            ColumnService.ColumnResizingEndedObservable.Where(c => c.Id == Model.Id).Subscribe(c => StateHasChanged());
            return base.OnInitializedAsync();
        }

        public string SortSymbol { get; private set; } = string.Empty;

        private void OnOrderInfoChanged(DataGridColumnOrderInfo args)
        {
            string sortSymbol = string.Empty;

            if (args.Field == Model.Field.Value && args.Asc)
            {
                sortSymbol = "&#x2BC5;";
            }
            else if (args.Field == Model.Field.Value && !args.Asc)
            {
                sortSymbol = "&#x2BC6;";
            }

            SortSymbol = sortSymbol;

            StateHasChanged();
        }

        protected string ColumnCellDraggable = "false";
		internal Task ColumnCellClick() => SortingService.OnColumnClick(Model);

        protected async Task OnMouseDown(MouseEventArgs args, IDataColumnModel columnMetadata)
        {
            await JsService.AttachWindowMouseMoveEvent(LayoutService.DataGridDotNetRef);
            await JsService.AttachWindowMouseUpEvent(LayoutService.DataGridDotNetRef);

            ColumnResizingService.IsMouseDown = true;
            ColumnResizingService.StartMouseMoveX = args.ClientX;
            ColumnResizingService.MouseMoveCol = columnMetadata;
            //ColumnResizingService.OldAbsoluteVisibleWidthValue = columnMetadata.VisibleAbsoluteWidthValue;

            IColumnModel colEmpty = ColumnService.EmptyColumnModel;
            //ColumnResizingService.OldAbsoluteEmptyColVisibleWidthValue = colEmpty.VisibleAbsoluteWidthValue;

            var rightSideColumn = ColumnService.AllColumnModels.SkipWhile(c => c.Id != columnMetadata.Id).Skip(1).FirstOrDefault();

            await JsService.StartDataGridColumnWidthChangeMode(
                ColumnResizingService.ColumnResizingDotNetRef,
                LayoutService.ActualColumnsWidthSum,
                columnMetadata.Id,
                ConstantService.InnerGridId,
                ConstantService.InnerGridBodyTableId,

                columnMetadata.VisibleGridColumnCellId.ToString(),
                columnMetadata.HiddenGridColumnCellId.ToString(),
                columnMetadata.FilterGridColumnCellId.ToString(),


                colEmpty.VisibleGridColumnCellId.ToString(),
                colEmpty.HiddenGridColumnCellId.ToString(),
                colEmpty.FilterGridColumnCellId.ToString(),

                ColumnService.EmptyColumnModel.Id,
                args.ClientX,
                columnMetadata.VisibleAbsoluteWidthValue,
                LayoutService.ColumnResizeMode.Value.ToString(),
                colEmpty.VisibleAbsoluteWidthValue,
                rightSideColumn?.Id,
                rightSideColumn?.VisibleGridColumnCellId,
                rightSideColumn?.VisibleAbsoluteWidthValue,
                rightSideColumn?.FilterGridColumnCellId,
                rightSideColumn?.HiddenGridColumnCellId);
        }

        private void ColumnCellMouseDown(MouseEventArgs e) => ColumnCellDraggable = "true";
        
        private void ColumnCellMouseUp(MouseEventArgs e) => ColumnCellDraggable = "false";

        public ValueTask DisposeAsync()
        {
            _columnOrderInfoUnsubscriber?.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
