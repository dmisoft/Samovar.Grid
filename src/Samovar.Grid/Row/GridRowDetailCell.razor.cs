using Microsoft.AspNetCore.Components;

namespace Samovar.Grid;

public partial class GridRowDetailCell<T>
    : DesignComponentBase, IAsyncDisposable
{
    [CascadingParameter(Name = "datagrid-row")]
    public required GridRow<T> GridRow { get; set; }

    [Parameter]
    public required GridRowModel<T> RowModel { get; set; }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
