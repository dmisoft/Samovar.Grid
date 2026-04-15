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
    protected string PaginationCssClass = "pagination";
    protected string _paginationSizeClass = "";

    protected async override Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        PagingNavigationStrategy.PagerInfo.Subscribe(this);
        LayoutService.CssClass.Subscribe(_ => { CssClass = _; });
        LayoutService.PaginationCssClass.Subscribe(_ =>
        {
            PaginationCssClass = string.IsNullOrWhiteSpace(_) ? "pagination" : $"pagination {_}";
        });
        LayoutService.SizeMode.Subscribe(mode => {
            _paginationSizeClass = mode switch { GridSizeMode.Small => "pagination-sm", GridSizeMode.Large => "pagination-lg", _ => "" };
            _ = InvokeAsync(StateHasChanged);
        });
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
