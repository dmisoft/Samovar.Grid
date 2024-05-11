using Microsoft.AspNetCore.Components;

namespace Samovar.Blazor
{
    public partial class SmDataGridRow<TItem>
        : SmDesignComponentBase, IAsyncDisposable
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [SmInject]
        protected IColumnService GridColumnService { get; set; }

        [SmInject]
        protected IGridStateService GridStateService { get; set; }

        [SmInject]
        protected ITemplateService<TItem> TemplateService { get; set; }

        [SmInject]
        protected IEditingService<TItem> EditingService { get; set; }

        [SmInject]
        protected IGridStateService StateService { get; set; }

        [SmInject]
        protected IRowDetailService<TItem> RowDetailService { get; set; }

        [SmInject]
        public IComponentBuilderService ComponentBuilderService { get; set; }

        [Parameter]
        public SmDataGridRowModel<TItem> RowModel { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [Parameter]
        public EventCallback<SmDataGridRowModel<TItem>> RowModelChanged { get; set; }

        internal async Task DetailExpanderClick()
        {
            RowModel.RowDetailExpanded = !RowModel.RowDetailExpanded;
            await RowDetailService.ExpandOrCloseRowDetails(RowModel.DataItem);
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
