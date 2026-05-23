namespace Samovar.Grid;

public partial class GridGroupIndentHeaderCell : DesignComponentBase, IAsyncDisposable
{
    [SmInject]
    public required IColumnService ColumnService { get; set; }

    protected string WidthStyle = "";

    protected override Task OnInitializedAsync()
    {
        ColumnService.GroupIndentColumnModel.WidthStyle.Subscribe(w =>
        {
            WidthStyle = w;
            _ = InvokeAsync(StateHasChanged);
        });
        return base.OnInitializedAsync();
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
