using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Samovar.Blazor.Header
{
    public partial class SmDataGridHeaderCell
        : SmDesignComponentBase, IAsyncDisposable
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [Parameter]
        public IDataColumnModel Model { get; set; }

        [SmInject]
        public ISortingService SortingService { get; set; }

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
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        IDisposable? _columnOrderInfoUnsubscriber = null;
        protected override Task OnInitializedAsync()
        {
            _columnOrderInfoUnsubscriber = SortingService.ColumnOrderInfo.Subscribe(OnOrderInfoChanged);
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

        internal Task ColumnCellClick()
        {
            return SortingService.OnColumnClick(Model);
        }

        protected async Task OnMouseDown(MouseEventArgs args, IDataColumnModel colMeta)
        {
            await JsService.AttachWindowMouseMoveEvent(LayoutService.DataGridDotNetRef);
            await JsService.AttachWindowMouseUpEvent(LayoutService.DataGridDotNetRef);

            ColumnResizingService.IsMouseDown = true;
            ColumnResizingService.StartMouseMoveX = args.ClientX;
            ColumnResizingService.MouseMoveCol = colMeta;
            ColumnResizingService.OldAbsoluteVisibleWidthValue = colMeta.VisibleAbsoluteWidthValue;

            IColumnModel colEmpty = ColumnService.EmptyColumnModel;
            ColumnResizingService.OldAbsoluteEmptyColVisibleWidthValue = colEmpty.VisibleAbsoluteWidthValue;

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

        public ValueTask DisposeAsync()
        {
            _columnOrderInfoUnsubscriber?.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
