using Microsoft.AspNetCore.Components;

namespace Samovar.Grid;

public partial class GridRowSelectionCell<T>
    : DesignComponentBase, IAsyncDisposable
{
    [SmInject]
    public required IColumnService ColumnService { get; set; }

    [SmInject]
    public required IGridSelectionService<T> GridSelectionService { get; set; }

    [Parameter]
    public required GridRowModel<T> RowModel { get; set; }

    protected string WidthStyle = "";

    protected override Task OnInitializedAsync()
    {
        ColumnService.RowSelectionColumnModel.WidthStyle.Subscribe(w =>
        {
            WidthStyle = w;
            _ = InvokeAsync(StateHasChanged);
        });
        return base.OnInitializedAsync();
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
