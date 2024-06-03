using Microsoft.AspNetCore.Components;

namespace Samovar.Grid.Edit
{
    public partial class GridRowEditing_Body<TItem>
        : DesignComponentBase, IAsyncDisposable
    {

        [Parameter]
        public required GridRowModel<TItem> RowModel { get; set; }

        public ValueTask DisposeAsync()
        {
            return new ValueTask(Task.CompletedTask);
        }
    }
}
