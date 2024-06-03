using Microsoft.AspNetCore.Components;

namespace Samovar.Grid.Edit
{
    public partial class GridRowInserting_Form<TItem>
        : DesignComponentBase, IAsyncDisposable
    {
        [SmInject]
        public required ILayoutService LayoutService { get; set; }

        [SmInject]
        public required IColumnService ColumnService { get; set; }

        [SmInject]
        public required IEditingService<TItem> EditingService { get; set; }

        [Parameter]
        public required GridRowModel<TItem> RowModel { get; set; }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
