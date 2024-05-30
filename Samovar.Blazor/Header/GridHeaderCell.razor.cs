using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Reactive.Linq;

namespace Samovar.Blazor.Header;

public partial class GridHeaderCell
    : SmDesignComponentBase, IAsyncDisposable
{
    [Parameter]
    public required IDataColumnModel ColumnModel { get; set; }

    [SmInject]
    public required ISortingService SortingService { get; set; }

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

    IDisposable? _columnOrderInfoUnsubscriber = null;
	protected string WidthStyle = "";

	protected override Task OnInitializedAsync()
    {
        _columnOrderInfoUnsubscriber = SortingService.ColumnOrderInfo.Subscribe(OnOrderInfoChanged);
		ColumnModel.WidthStyle.Subscribe(w =>
		{
			WidthStyle = w;
            StateHasChanged();
		});
		return base.OnInitializedAsync();
    }

    public string SortSymbol { get; private set; } = string.Empty;

    private void OnOrderInfoChanged(DataGridColumnOrderInfo args)
    {
        string sortSymbol = string.Empty;

        if (args.Field == ColumnModel.Field.Value && args.Asc)
        {
            sortSymbol = "&#x2BC5;";
        }
        else if (args.Field == ColumnModel.Field.Value && !args.Asc)
        {
            sortSymbol = "&#x2BC6;";
        }

        SortSymbol = sortSymbol;

        StateHasChanged();
    }

    protected string ColumnCellDraggable = "false";
		internal Task ColumnCellClick() => SortingService.OnColumnClick(ColumnModel);

    protected async Task OnMouseDown(MouseEventArgs args, IDataColumnModel triggerColumnModel)
    {
        await JsService.AttachWindowMouseMoveEvent(LayoutService.DataGridDotNetRef);
        await JsService.AttachWindowMouseUpEvent(LayoutService.DataGridDotNetRef);

        ColumnResizingService.IsMouseDown = true;
        ColumnResizingService.StartMouseMoveX = args.ClientX;
        ColumnResizingService.MouseMoveCol = triggerColumnModel;

        IColumnModel emptyHeaderColumnModel = ColumnService.EmptyColumnModel;

        var rightSideColumn = ColumnService.AllColumnModels.SkipWhile(c => c.Id != triggerColumnModel.Id).Skip(1).FirstOrDefault();

        await JsService.StartDataGridColumnWidthChangeMode(
            ColumnResizingService.ColumnResizingDotNetRef,
            LayoutService.ActualColumnsWidthSum,
            triggerColumnModel.Id,
            ConstantService.InnerGridId,
            ConstantService.InnerGridBodyTableId,

            triggerColumnModel.HeaderCellId.ToString(),
            triggerColumnModel.HiddenHeaderCellId.ToString(),
            triggerColumnModel.FilterCellId.ToString(),

            emptyHeaderColumnModel.HeaderCellId.ToString(),
            emptyHeaderColumnModel.HiddenHeaderCellId.ToString(),
            emptyHeaderColumnModel.FilterCellId.ToString(),

            ColumnService.EmptyColumnModel.Id,
            args.ClientX,
            triggerColumnModel.Width.Value,
            LayoutService.ColumnResizeMode.Value.ToString(),
            emptyHeaderColumnModel.Width.Value,
            rightSideColumn?.Id,
            rightSideColumn?.HeaderCellId,
            rightSideColumn?.Width.Value,
            rightSideColumn?.FilterCellId,
            rightSideColumn?.HiddenHeaderCellId,
            ConstantService.OuterGridId
				);
    }

    private void ColumnCellMouseDown(MouseEventArgs e) => ColumnCellDraggable = "true";
    
    private void ColumnCellMouseUp(MouseEventArgs e) => ColumnCellDraggable = "false";

    public ValueTask DisposeAsync()
    {
        _columnOrderInfoUnsubscriber?.Dispose();
        return ValueTask.CompletedTask;
    }
}
