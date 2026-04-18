namespace Samovar.Grid;

public partial class NoDataPanel
    : DesignComponentBase, IAsyncDisposable
{
    protected double ContainerHeight = 0;

    private IDisposable? _cultureSubscription;

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            ContainerHeight = 330;

        return base.OnAfterRenderAsync(firstRender);
    }

    protected override Task OnInitializedAsync()
    {
        _cultureSubscription = L10n.CultureChanged.Subscribe(u => InvokeAsync(StateHasChanged));
        return base.OnInitializedAsync();
    }

    public ValueTask DisposeAsync()
    {
        _cultureSubscription?.Dispose();
        _cultureSubscription = null;
        return ValueTask.CompletedTask;
    }
}
