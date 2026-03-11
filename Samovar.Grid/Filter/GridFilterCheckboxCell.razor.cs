namespace Samovar.Grid.Filter;

public partial class GridFilterCheckboxCell : DesignComponentBase, IAsyncDisposable
{
    [SmInject]
    public required ILayoutService LayoutService { get; set; }

    [SmInject]
    public required IColumnService ColumnService { get; set; }

    protected string WidthStyle = "";

    protected override Task OnInitializedAsync()
    {
        ColumnService.RowSelectionColumnModel.WidthStyle.Subscribe(w =>
        {
            WidthStyle = w;
            StateHasChanged();
        });
        return base.OnInitializedAsync();
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
