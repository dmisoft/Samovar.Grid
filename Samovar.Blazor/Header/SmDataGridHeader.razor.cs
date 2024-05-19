using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Samovar.Blazor.Header
{
    public partial class SmDataGridHeader<TItem>
        : SmDesignComponentBase, IAsyncDisposable
    {
        [Inject]
        public required IJSRuntime JsRuntime { get; set; }

        [SmInject]
        public required IColumnService GridColumnService { get; set; }

        [SmInject]
        public required ILayoutService LayoutService { get; set; }

        [SmInject]
        public required ISortingService GridOrderService { get; set; }

        [SmInject]
        public required IEditingService<TItem> GridEditingService { get; set; }

        [SmInject]
        public required IGridStateService GridStateService { get; set; }

        protected Task RowInsering()
        {
            return GridEditingService.RowInsertBegin();
        }

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
