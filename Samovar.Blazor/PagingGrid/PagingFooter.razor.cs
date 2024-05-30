using Microsoft.AspNetCore.Components;

namespace Samovar.Blazor;

public partial class PagingFooter
    : SmDesignComponentBase, IAsyncDisposable, IObserver<DataGridPagerInfo>
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

    public void OnNext(DataGridPagerInfo value)
    {
        InvokeAsync(StateHasChanged);
    }

    public ValueTask DisposeAsync()
    {
        return new ValueTask();
    }
}
