using Microsoft.AspNetCore.Components;
using System.Diagnostics;

namespace Samovar.Blazor
{
    public partial class DataGridPagingTablePanel<T>
        : SmDesignComponentBase, IAsyncDisposable
    {
        [SmInject]
        public required IRepositoryService<T> RepositoryService { get; set; }

        [SmInject]
        public required ILayoutService LayoutService { get; set; }

        [SmInject]
        public required IEditingService<T> EditingService { get; set; }

        [SmInject]
        public required IComponentBuilderService ComponentBuilderService { get; set; }

        [SmInject]
        public required IConstantService ConstantService { get; set; }

        [SmInject]
        public required IGridStateService StateService { get; set; }


        [SmInject]
        public required INavigationService NavigationService { get; set; }

        public EventCallback<DataSourceState> DataSourceStateEv { get; set; }

        public DataGridStyleInfo? Style { get; set; }

        public ElementReference GridBodyRef { get; set; }

        protected IEnumerable<GridRowModel<T>> View { get; set; } = [];

        public DataSourceState DataSourceState { get; set; } = DataSourceState.NoData;

        public EventCallback<IEnumerable<GridRowModel<T>>> CollectionViewChangedEv { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            await LayoutService.InitHeader();
        }

        protected override Task OnInitializedAsync()
        {
            SubscribeViewCollectionChange();

            StateService.DataSourceState.Subscribe(async (stateTask) =>
            {
                await InvokeAsync(async () =>
                {
                    DataSourceState = await stateTask;
                    StateHasChanged();
                });
            });
            Style = new DataGridStyleInfo
            {
                CssStyle = LayoutService.OuterStyle.Value,
                ActualScrollbarWidth = 0// LayoutService.ActualScrollbarWidth
            };

            return base.OnInitializedAsync();
        }

        private void SubscribeViewCollectionChange()
        {
            RepositoryService.ViewCollectionObservableTask.Subscribe(async (GetViewCollectionTask) =>
            {
                View = await GetViewCollectionTask;
            });
        }

        public ValueTask DisposeAsync()
        {
            StateService.DataSourceStateEvList.Remove(DataSourceStateEv);
            RepositoryService.CollectionViewChangedEvList.Remove(CollectionViewChangedEv);
            return ValueTask.CompletedTask;
        }
    }
}
