namespace Samovar.Grid.Header;

public partial class GridHiddenHeader
    : DesignComponentBase, IAsyncDisposable
{
    [SmInject]
    public required IColumnService GridColumnService { get; set; }

    [SmInject]
    public required ILayoutService GridLayoutService { get; set; }

    protected override Task OnInitializedAsync()
    {
        GridLayoutService.ShowRowSelectionColumn.Subscribe(v => _ = InvokeAsync(StateHasChanged));
        GridLayoutService.ActiveGroupCount.Subscribe(v => _ = InvokeAsync(StateHasChanged));
        return base.OnInitializedAsync();
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
