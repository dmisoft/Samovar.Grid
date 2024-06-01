using Microsoft.AspNetCore.Components;

namespace Samovar.Blazor;

public partial class PagingFooter
    : DesignComponentBase, IAsyncDisposable, IObserver<GridPagerInfo>
{
    [SmInject]
    public required ILayoutService LayoutService { get; set; }

    [SmInject]
    public required IPagingNavigationStrategy PagingNavigationStrategy { get; set; }

    internal ElementReference GridFooterRef { get; set; }

    protected override void OnInitialized()
    {
        PagingNavigationStrategy.PagerInfo.Subscribe(this);
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
