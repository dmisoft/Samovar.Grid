using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace Samovar.Blazor
{
    public partial class DataGridVirtualTablePanel<T>
        : SmDesignComponentBase, IAsyncDisposable
    {
        [SmInject]
        public required IGridStateService StateService { get; set; }


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
        public required INavigationService NavigationService { get; set; }

        [SmInject]
        public required IGridStateService GridStateService { get; set; }

        public EventCallback<DataSourceState> DataSourceStateEv { get; set; }

        public DataGridStyleInfo? Style { get; set; }

        public ElementReference GridBodyRef { get; set; }

        protected IEnumerable<GridRowModel<T>> View { get; set; } = [];

        private Task _collectionViewChangedEv(IEnumerable<GridRowModel<T>> collectionView)
        {
            View = collectionView;
            return Task.CompletedTask;
        }

        public EventCallback<IEnumerable<GridRowModel<T>>> CollectionViewChangedEv { get; set; }

        public DataSourceState DataSourceState { get; set; } = DataSourceState.NoData;

        private Virtualize<GridRowModel<T>>? virtualizeComponent;

        protected override Task OnInitializedAsync()
        {
            SubscribeViewCollectionChange();

            StateService.DataSourceState.Subscribe(async (stateTask) =>
            {
                await InvokeAsync(async () => {
                    DataSourceState = await stateTask;
                    StateHasChanged();
                });
            });
            Style = new DataGridStyleInfo
            {
                CssStyle = LayoutService.OuterStyle.Value,
                ActualScrollbarWidth = 0// LayoutService.ActualScrollbarWidth
            };

            CollectionViewChangedEv = new EventCallbackFactory().Create<IEnumerable<GridRowModel<T>>>(this, async (data) => await _collectionViewChangedEv(data));
            RepositoryService.CollectionViewChangedEvList.Add(CollectionViewChangedEv);

            return base.OnInitializedAsync();
        }

        private void SubscribeViewCollectionChange()
        {
            RepositoryService.ViewCollectionObservableTask.Subscribe(async (GetViewCollectionTask) =>
            {
                View = await GetViewCollectionTask;
                await InvokeAsync(async () =>
                {
                    if (virtualizeComponent is not null)
                    {
                        await virtualizeComponent.RefreshDataAsync();
                        StateHasChanged();
                    }
                });
            });
        }
        public ValueTask DisposeAsync()
        {
            GridStateService.DataSourceStateEvList.Remove(DataSourceStateEv);
            RepositoryService.CollectionViewChangedEvList.Remove(CollectionViewChangedEv);
            return ValueTask.CompletedTask;
        }
    }
}
