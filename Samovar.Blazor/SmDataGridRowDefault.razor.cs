using Microsoft.AspNetCore.Components;

namespace Samovar.Blazor
{
    public partial class SmDataGridRowDefault<T>
        : SmDesignComponentBase, IAsyncDisposable
    {
        [CascadingParameter(Name = "datagrid-row")]
        public required SmDataGridRow<T> GridRow { get; set; }

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
        public required IRowDetailService<T> RowDetailService { get; set; }

        [Parameter]
        public required GridRowModel<T> RowModel { get; set; }

        IDisposable? SingleSelectedDataRowsSubscription = null;

        IDisposable? MultipleSelectedDataRowsSubscription = null;

        protected override Task OnParametersSetAsync()
        {
            base.OnParametersSetAsync();
            RowModel.RowSelected = false;
            if (GridSelectionService is null)
            {
                return Task.CompletedTask;
            }

            switch (GridSelectionService.SelectionMode.Value)
            {
                case GridSelectionMode.None:
                    break;
                case GridSelectionMode.Single:
                    var val = GridSelectionService.SingleSelectedDataRow.Value;
                    if (val is null)
                        break;
                    RowModel.RowSelected = !object.Equals(val, default(T)) && val.Equals(RowModel.DataItem);
                    break;
                case GridSelectionMode.Multiple:
                    RowModel.RowSelected = GridSelectionService.MultipleSelectedDataRows.Value is not null && GridSelectionService.MultipleSelectedDataRows.Value.Any(a => a!.Equals(RowModel.DataItem));
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
                await EditingService.RowEditCancel();

            await EditingService.RowEditBegin(rowMainModel);
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
            RowModel.RowSelected = arg is not null && arg.Any(a => a!.Equals(RowModel.DataItem));
            StateHasChanged();
        }

        private void SingleSelectedDataRowsChanged(T? arg)
        {
            RowModel.RowSelected = arg is not null && arg.Equals(RowModel.DataItem);
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
}
