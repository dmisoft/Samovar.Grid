namespace Samovar.Grid;

public partial class GridSummaryEmptyCell
    : DesignComponentBase, IAsyncDisposable
{
    [SmInject]
    public required IColumnService ColumnService { get; set; }

    protected string WidthStyle = "";
    private IDisposable? _widthSub;

    protected override Task OnInitializedAsync()
    {
        _widthSub = ColumnService.EmptyColumnModel.WidthStyle.Subscribe(w =>
        {
            WidthStyle = w;
            _ = InvokeAsync(StateHasChanged);
        });
        return base.OnInitializedAsync();
    }

    public ValueTask DisposeAsync()
    {
        _widthSub?.Dispose();
        return ValueTask.CompletedTask;
    }
}
