using Microsoft.AspNetCore.Components;
using System.Reactive.Linq;

namespace Samovar.Blazor.Header;

public partial class GridHeaderCommandCell<T>
		: SmDesignComponentBase, IAsyncDisposable
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

	protected override Task OnInitializedAsync()
    {
        //ColumnService.ColumnResizingEndedObservable.Where(c => c.Id == ColumnModel.Id).Subscribe(c => StateHasChanged());
		ColumnModel.WidthStyle.Subscribe(w => {
			WidthStyle = w;
            StateHasChanged();
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
