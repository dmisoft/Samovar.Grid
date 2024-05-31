using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Samovar.Blazor.Header;

public partial class GridHeader<T>
    : SmDesignComponentBase, IAsyncDisposable
{
    [Inject]
    public required IJSRuntime JsRuntime { get; set; }

    [SmInject]
    public required IConstantService ConstantService { get; set; }

    [SmInject]
    public required IColumnService GridColumnService { get; set; }

    [SmInject]
    public required ILayoutService LayoutService { get; set; }

    [SmInject]
    public required ISortingService GridOrderService { get; set; }

    [SmInject]
    public required IGridStateService GridStateService { get; set; }

    internal Task ColumnCellClick(IDataColumnModel columnModel)
    {
        return GridOrderService.OnColumnClick(columnModel);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
