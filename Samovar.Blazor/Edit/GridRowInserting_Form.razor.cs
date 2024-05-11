using Microsoft.AspNetCore.Components;

namespace Samovar.Blazor.Edit
{
    public partial class GridRowInserting_Form<TItem>
        : SmDesignComponentBase, IAsyncDisposable
    {
        [SmInject]
        public required ILayoutService LayoutService { get; set; }

        [SmInject]
        public required IColumnService ColumnService { get; set; }

        [SmInject]
        public required IEditingService<TItem> EditingService { get; set; }

        [Parameter]
        public required SmDataGridRowModel<TItem> RowModel { get; set; }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
