using Microsoft.AspNetCore.Components;

namespace Samovar.Grid;

public partial class GridRowDefault<T>
    : DesignComponentBase, IAsyncDisposable
{
    [CascadingParameter(Name = "datagrid-row")]
    public required GridRow<T> GridRow { get; set; }

    [SmInject]
    public required IColumnService ColumnService { get; set; }

    [SmInject]
    public required ILayoutService LayoutService { get; set; }

    [SmInject]
    public required IGridStateService GridStateService { get; set; }

    [SmInject]
    public required IEditingService<T> EditingService { get; set; }

    [SmInject]
    public required ITemplateService<T> TemplateService { get; set; }

    [SmInject]
    public required IGridSelectionService<T> GridSelectionService { get; set; }

    [SmInject]
    public required IDetailRowService<T> RowDetailService { get; set; }

    [Parameter]
    public required GridRowModel<T> RowModel { get; set; }

    IDisposable? SingleSelectedDataRowsSubscription = null;

    IDisposable? MultipleSelectedDataRowsSubscription = null;

    protected override Task OnParametersSetAsync()
    {
        base.OnParametersSetAsync();
        RowModel.IsRowSelected = false;
        //subscribtion am besten in row model?!
        switch (GridSelectionService.SelectionMode.Value)
        {
            case RowSelectionMode.None:
                break;
            case RowSelectionMode.Single:
                var val = GridSelectionService.SingleSelectedDataRow.Value;
                if (val is null)
                    break;
                RowModel.IsRowSelected = !object.Equals(val, default(T)) && val.Equals(RowModel.DataItem);
                break;
            case RowSelectionMode.Multiple:
                RowModel.IsRowSelected = GridSelectionService.MultipleSelectedDataRows.Value is not null && GridSelectionService.MultipleSelectedDataRows.Value.Any(a => a!.Equals(RowModel.DataItem));
                break;
            default:
                break;
        }

        return Task.CompletedTask;
    }

    // Row editing
    protected async Task RowEditBegin(GridRowModel<T> rowMainModel)
    {
        var actualState = await GridStateService.DataSourceState.Value;
        if (actualState != DataSourceState.Idle)
            await EditingService.CancelRowEdit(rowMainModel);

        await EditingService.EditBegin(rowMainModel);
    }

    // Row deleting
    protected async Task RowDeleteBegin(GridRowModel<T> rowMainModel)
    {
        await EditingService.RowDeleteBegin(rowMainModel);
    }

    internal async Task RowSelectedIntern(GridRowEventArgs args, GridRowModel<T> selectedModel)
    {
        await GridSelectionService.OnRowSelected(selectedModel.DataItem);
    }

    protected override Task OnInitializedAsync()
    {
        SingleSelectedDataRowsSubscription = GridSelectionService.SingleSelectedDataRow.Subscribe(SingleSelectedDataRowsChanged);
        MultipleSelectedDataRowsSubscription = GridSelectionService.MultipleSelectedDataRows.Subscribe(MultipleSelectedDataRowsChanged);
        EditingService.RowEditingEnded += EditingService_RowEditingEnded;

        return base.OnInitializedAsync();
    }

    private Task EditingService_RowEditingEnded()
    {
        StateHasChanged();
        return Task.CompletedTask;
    }

    private void MultipleSelectedDataRowsChanged(IEnumerable<T>? arg)
    {
        RowModel.IsRowSelected = arg is not null && arg.Any(a => a!.Equals(RowModel.DataItem));
        StateHasChanged();
    }

    private void SingleSelectedDataRowsChanged(T? arg)
    {
        RowModel.IsRowSelected = arg is not null && arg.Equals(RowModel.DataItem);
        StateHasChanged();
    }

    public ValueTask DisposeAsync()
    {
        SingleSelectedDataRowsSubscription?.Dispose();
        MultipleSelectedDataRowsSubscription?.Dispose();
        EditingService.RowEditingEnded -= EditingService_RowEditingEnded;
        return ValueTask.CompletedTask;
    }
}
