using Microsoft.AspNetCore.Components;
using System.Reactive.Linq;

namespace Samovar.Grid.Header;

public partial class GridHeaderCommandCell<T>
        : DesignComponentBase, IAsyncDisposable
{
    [Parameter]
    public required IColumnModel ColumnModel { get; set; }

    [SmInject]
    public required ILayoutService LayoutService { get; set; }

    [SmInject]
    public required IColumnResizingService ColumnResizingService { get; set; }

    [SmInject]
    public required IJsService JsService { get; set; }

    [SmInject]
    public required IColumnService ColumnService { get; set; }

    [SmInject]
    public required IConstantService ConstantService { get; set; }

    [SmInject]
    public required IFilterService FilterService { get; set; }

    [SmInject]
    public required IEditingService<T> EditingService { get; set; }


    protected string WidthStyle = "";
    protected string _btnSizeClass = "";

    protected override Task OnInitializedAsync()
    {
        ColumnModel.WidthStyle.Subscribe(w =>
        {
            WidthStyle = w;
            _ = InvokeAsync(StateHasChanged);
        });
        LayoutService.SizeMode.Subscribe(mode =>
        {
            _btnSizeClass = mode switch { GridSizeMode.Small => "btn-sm small", GridSizeMode.Large => "btn-lg", _ => "" };
            _ = InvokeAsync(StateHasChanged);
        });
        return base.OnInitializedAsync();
    }

    protected string ColumnCellDraggable = "false";
    protected Task RowInsering()
    {
        return EditingService.RowInsertBegin();
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
