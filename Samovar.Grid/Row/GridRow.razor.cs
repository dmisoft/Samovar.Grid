using Microsoft.AspNetCore.Components;
using System.Reactive.Linq;

namespace Samovar.Grid;

public partial class GridRow<TItem>
    : DesignComponentBase, IAsyncDisposable
{
    //[SmInject]
    //public required IColumnService GridColumnService { get; set; }

    //[SmInject]
    //public required IGridStateService GridStateService { get; set; }

    //[SmInject]
    //public required ITemplateService<TItem> TemplateService { get; set; }

    //[SmInject]
    //public required IEditingService<TItem> EditingService { get; set; }

    //[SmInject]
    //public required IGridStateService StateService { get; set; }

    [SmInject]
    public required IRowDetailService<TItem> RowDetailService { get; set; }

    [SmInject]
    public required IComponentBuilderService ComponentBuilderService { get; set; }

    [Parameter]
    public required GridRowModel<TItem> RowModel { get; set; }

    //[Parameter]
    //public EventCallback<GridRowModel<TItem>> RowModelChanged { get; set; }

    IDisposable? rowStateSubscriber;

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        await base.SetParametersAsync(parameters);

        rowStateSubscriber?.Dispose();
        var rowModel = parameters.GetValueOrDefault<GridRowModel<TItem>>(nameof(RowModel));
        if (rowModel is not null)
            rowStateSubscriber = rowModel.RowState.Subscribe(async (state) =>
            {
                await InvokeAsync(StateHasChanged);
            });
    }

    internal async Task DetailExpanderClick()
    {
        RowModel.RowDetailExpanded = !RowModel.RowDetailExpanded;
        await RowDetailService.ExpandOrCloseRowDetails(RowModel.DataItem);
    }

    public ValueTask DisposeAsync()
    {
        rowStateSubscriber?.Dispose();
        return ValueTask.CompletedTask;
    }
}
