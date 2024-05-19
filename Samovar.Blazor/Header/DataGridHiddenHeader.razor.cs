namespace Samovar.Blazor.Header;

public partial class DataGridHiddenHeader
    : SmDesignComponentBase, IAsyncDisposable
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [SmInject]
    protected IColumnService GridColumnService { get; set; }

    [SmInject]
    protected ILayoutService GridLayoutService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
