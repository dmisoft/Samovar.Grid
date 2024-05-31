namespace Samovar.Blazor.Header;

public partial class GridHiddenHeader
    : SmDesignComponentBase, IAsyncDisposable
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
