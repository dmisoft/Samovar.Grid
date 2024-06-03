namespace Samovar.Grid.Header;

public partial class GridHiddenHeader
    : DesignComponentBase, IAsyncDisposable
{
    [SmInject]
    public required IColumnService GridColumnService { get; set; }

    [SmInject]
    public required ILayoutService GridLayoutService { get; set; }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
