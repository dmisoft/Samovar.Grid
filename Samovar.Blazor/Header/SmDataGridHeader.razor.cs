using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Samovar.Blazor.Header
{
    public partial class SmDataGridHeader<TItem>
        : SmDesignComponentBase, IAsyncDisposable
    {

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        [SmInject]
        protected IColumnService GridColumnService { get; set; }

        [SmInject]
        protected ILayoutService GridLayoutService { get; set; }

        [SmInject]
        protected ISortingService GridOrderService { get; set; }

        [SmInject]
        protected IEditingService<TItem> GridEditingService { get; set; }

        [SmInject]
        protected IGridStateService GridStateService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

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
