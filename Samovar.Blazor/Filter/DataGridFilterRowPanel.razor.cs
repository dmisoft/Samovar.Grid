namespace Samovar.Blazor.Filter;

public partial class DataGridFilterRowPanel<T>
    : SmDesignComponentBase, IAsyncDisposable
{
    [SmInject]
    public required ILayoutService GridLayoutService { get; set; }

    [SmInject]
    public required IConstantService ConstantService { get; set; }

    public DataGridStyleInfo? Style { get; set; }

    protected override void OnInitialized()
    {
        GridLayoutService.DataGridInnerStyle.Subscribe(async style => {
            Style = await style;
            StateHasChanged();
        });
        base.OnInitialized();
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}