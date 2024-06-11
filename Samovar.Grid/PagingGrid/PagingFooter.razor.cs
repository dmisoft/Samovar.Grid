using Microsoft.AspNetCore.Components;

namespace Samovar.Grid;

public partial class PagingFooter
    : DesignComponentBase, IAsyncDisposable, IObserver<GridPagerInfo>
{
    [SmInject]
    public required ILayoutService LayoutService { get; set; }

    [SmInject]
    public required IPagingNavigationStrategy PagingNavigationStrategy { get; set; }

    internal ElementReference GridFooterRef { get; set; }

    protected string CssClass = "";

    protected async override Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        PagingNavigationStrategy.PagerInfo.Subscribe(this);
        LayoutService.CssClass.Subscribe(_ => { CssClass = _; });
    }

    public void OnCompleted()
    {
        throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public void OnNext(GridPagerInfo value)
    {
        InvokeAsync(StateHasChanged);
    }

    public ValueTask DisposeAsync()
    {
        return new ValueTask();
    }
}
