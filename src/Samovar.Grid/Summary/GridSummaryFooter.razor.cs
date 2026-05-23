namespace Samovar.Grid;

public partial class GridSummaryFooter<T>
    : DesignComponentBase, IAsyncDisposable
{
    [SmInject]
    public required IConstantService ConstantService { get; set; }

    [SmInject]
    public required IColumnService ColumnService { get; set; }

    [SmInject]
    public required ILayoutService LayoutService { get; set; }

    [SmInject]
    public required ISummaryFooterService SummaryFooterService { get; set; }

    protected string CssClass = "";
    protected string _tableSizeClass = "";

    private IDisposable? _cssClassSub;
    private IDisposable? _sizeModeSub;
    private IDisposable? _minGridWidthSub;
    private IDisposable? _showRowSelectionSub;
    private IDisposable? _showDetailRowSub;
    private IDisposable? _valuesSub;
    private IDisposable? _cultureSub;
    private IDisposable? _activeGroupCountSub;

    protected override Task OnInitializedAsync()
    {
        _cssClassSub = LayoutService.CssClass.Subscribe(c => { CssClass = c; _ = InvokeAsync(StateHasChanged); });
        _sizeModeSub = LayoutService.SizeMode.Subscribe(mode =>
        {
            _tableSizeClass = mode switch
            {
                GridSizeMode.Small => "table-sm small",
                GridSizeMode.Large => "sm-table-lg",
                _ => ""
            };
            _ = InvokeAsync(StateHasChanged);
        });
        _minGridWidthSub = LayoutService.MinGridWidth.Subscribe(_ => InvokeAsync(StateHasChanged));
        _showRowSelectionSub = LayoutService.ShowRowSelectionColumn.Subscribe(_ => InvokeAsync(StateHasChanged));
        _showDetailRowSub = LayoutService.ShowDetailRow.Subscribe(_ => InvokeAsync(StateHasChanged));
        _valuesSub = SummaryFooterService.Values.Subscribe(_ => InvokeAsync(StateHasChanged));
        _cultureSub = L10n.CultureChanged.Subscribe(_ => InvokeAsync(StateHasChanged));
        _activeGroupCountSub = LayoutService.ActiveGroupCount.Subscribe(_ => InvokeAsync(StateHasChanged));
        return base.OnInitializedAsync();
    }

    public ValueTask DisposeAsync()
    {
        _cssClassSub?.Dispose();
        _sizeModeSub?.Dispose();
        _minGridWidthSub?.Dispose();
        _showRowSelectionSub?.Dispose();
        _showDetailRowSub?.Dispose();
        _valuesSub?.Dispose();
        _cultureSub?.Dispose();
        _activeGroupCountSub?.Dispose();
        return ValueTask.CompletedTask;
    }
}
