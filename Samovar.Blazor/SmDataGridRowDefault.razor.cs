using Microsoft.AspNetCore.Components;

namespace Samovar.Blazor
{
    public partial class SmDataGridRowDefault<T>
        : SmDesignComponentBase, IAsyncDisposable
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [CascadingParameter(Name = "datagrid-row")]
        protected SmDataGridRow<T> GridRow { get; set; }

        [SmInject]
        public IColumnService ColumnService { get; set; }

        [SmInject]
        public ILayoutService LayoutService { get; set; }

        [SmInject]
        public IGridStateService GridStateService { get; set; }

        [SmInject]
        public IEditingService<T> EditingService { get; set; }

        [SmInject]
        public ITemplateService<T> TemplateService { get; set; }

        [SmInject]
        public IGridSelectionService<T> GridSelectionService { get; set; }

        [SmInject]
        protected IRowDetailService<T> RowDetailService { get; set; }

        [Parameter]
        public SmDataGridRowModel<T> RowModel { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        IDisposable? SingleSelectedDataRowsSubscription = null;

        IDisposable? MultipleSelectedDataRowsSubscription = null;

        protected override Task OnParametersSetAsync()
        {
            base.OnParametersSetAsync();
            RowModel.RowSelected = false;
            if(GridSelectionService is null)
            {
                return Task.CompletedTask;
            }

            switch (GridSelectionService.SelectionMode.Value)
            {
                case GridSelectionMode.None:
                    break;
                case GridSelectionMode.SingleSelectedDataRow:
                    var val = GridSelectionService.SingleSelectedDataRow.Value;
                    if (val is null)
                        break;
                    RowModel.RowSelected = !object.Equals(val, default(T)) && val.Equals(RowModel.DataItem);
                    break;
                case GridSelectionMode.MultipleSelectedDataRows:
                    RowModel.RowSelected = GridSelectionService.MultipleSelectedDataRows.Value is not null && GridSelectionService.MultipleSelectedDataRows.Value.Any(a => a!.Equals(RowModel.DataItem));
                    break;
                default:
                    break;
            }

            return Task.CompletedTask;
        }

        // Row editing
        protected async Task RowEditBegin(SmDataGridRowModel<T> rowMainModel)
        {
            if (GridStateService.DataSourceState.Value != DataSourceState.Idle)
                await EditingService.RowEditCancel();

            await EditingService.RowEditBegin(rowMainModel);
        }

        // Row deleting
        protected async Task RowDeleteBegin(SmDataGridRowModel<T> rowMainModel)
        {
            await EditingService.RowDeleteBegin(rowMainModel);
        }

        internal async Task RowSelectedIntern(GridRowEventArgs args, SmDataGridRowModel<T> selectedModel)
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

        private void MultipleSelectedDataRowsChanged(IEnumerable<T> arg)
        {
            RowModel.RowSelected = arg is not null && arg.Any(a => a!.Equals(RowModel.DataItem));
            StateHasChanged();
        }

        private void SingleSelectedDataRowsChanged(T arg)
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
