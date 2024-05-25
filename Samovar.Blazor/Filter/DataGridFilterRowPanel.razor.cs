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
        //Style = new DataGridStyleInfo
        //{
        //    CssStyle = GridLayoutService.OuterStyle.Value,
        //    ActualScrollbarWidth = GridLayoutService.ActualScrollbarWidth!
        //};
        GridLayoutService.DataGridInnerStyle.Subscribe(async style => {
            Style = await style;
            StateHasChanged();
        });
        base.OnInitialized();
    }

    private Task GridLayoutService_DataGridInnerCssStyleChanged(DataGridStyleInfo arg)
    {
        Style = arg;
        InvokeAsync(StateHasChanged);
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        //GridLayoutService.DataGridInnerStyle -= GridLayoutService_DataGridInnerCssStyleChanged;
        return ValueTask.CompletedTask;
    }
}