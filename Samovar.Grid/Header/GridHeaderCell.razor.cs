using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Reactive.Linq;
using System.Globalization;

namespace Samovar.Grid.Header;

public partial class GridHeaderCell<TItem>
    : DesignComponentBase, IAsyncDisposable
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

    [SmInject]
    public required IFilterService FilterService { get; set; }

    private bool _filterMenuOpen = false;
    private bool _filterActive = false;
    private double _filterMenuTop = 0;
    private double _filterMenuLeft = 0;
    private ElementReference _filterButtonRef;
    IDisposable? _filterInfoUnsubscriber = null;
    IDisposable? _activeFilterMenuUnsubscriber = null;

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
        _filterInfoUnsubscriber = FilterService.FilterInfo.Subscribe(filters =>
        {
            _filterActive = filters.Any(f => f.ColumnModel is not null && f.ColumnModel.Equals(ColumnModel));
            StateHasChanged();
        });
        _activeFilterMenuUnsubscriber = FilterService.ActiveFilterMenuColumnId.Subscribe(activeId =>
        {
            if (_filterMenuOpen && activeId != ColumnModel.Id)
            {
                _filterMenuOpen = false;
                StateHasChanged();
            }
        });
        return base.OnInitializedAsync();
    }

    internal enum SortDir { None, Asc, Desc }

    internal SortDir SortDirection { get; private set; } = SortDir.None;

    private void OnOrderInfoChanged(ColumnOrderInfo args)
    {
        if (args.Field == ColumnModel.Field.Value)
        {
            SortDirection = args.Asc ? SortDir.Asc : SortDir.Desc;
        }
        else
        {
            SortDirection = SortDir.None;
        }

        StateHasChanged();
    }

    private bool IsLastColumn => ColumnService.AllColumnModels[^1].Id == ColumnModel.Id;

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
            ConstantService.OuterGridId,
            triggerColumnModel.MinWidth,
            (rightSideColumn as IDeclarativeColumnModel)?.MinWidth ?? 50d
                );
    }

    private void ColumnCellMouseDown(MouseEventArgs e) => ColumnCellDraggable = "true";

    private void ColumnCellMouseUp(MouseEventArgs e) => ColumnCellDraggable = "false";

    private async Task ToggleFilterMenu()
    {
        _filterMenuOpen = !_filterMenuOpen;
        if (_filterMenuOpen)
        {
            var rect = await JsService.GetElementBoundingRect(_filterButtonRef);
            _filterMenuTop = rect.Top + rect.Height;
            _filterMenuLeft = rect.Left;
            FilterService.ActiveFilterMenuColumnId.OnNext(ColumnModel.Id);
        }
        else
        {
            FilterService.ActiveFilterMenuColumnId.OnNext(null);
        }
        StateHasChanged();
    }

    private void CloseFilterMenu()
    {
        _filterMenuOpen = false;
        FilterService.ActiveFilterMenuColumnId.OnNext(null);
        StateHasChanged();
    }

    public ValueTask DisposeAsync()
    {
        _columnOrderInfoUnsubscriber?.Dispose();
        _filterInfoUnsubscriber?.Dispose();
        _activeFilterMenuUnsubscriber?.Dispose();
        return ValueTask.CompletedTask;
    }
}
