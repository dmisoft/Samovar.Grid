﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using System.Diagnostics;

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
        public required IVirtualScrollingNavigationStrategy VirtualScrollingService { get; set; }

        [SmInject]
        public required IGridStateService GridStateService { get; set; }

        public EventCallback<DataSourceState> DataSourceStateEv { get; set; }

        public DataGridStyleInfo? Style { get; set; }

        public ElementReference GridBodyRef { get; set; }

        protected IEnumerable<SmDataGridRowModel<T>> View { get; set; } = [];

        private Task _collectionViewChangedEv(IEnumerable<SmDataGridRowModel<T>> collectionView)
        {
            View = collectionView;
            return Task.CompletedTask;
        }

        public EventCallback<IEnumerable<SmDataGridRowModel<T>>> CollectionViewChangedEv { get; set; }

        public DataSourceState dataSourceState = DataSourceState.NoData;
        private Virtualize<SmDataGridRowModel<T>>? virtualizeComponent;

        protected override Task OnInitializedAsync()
        {
            SubscribeViewCollectionChange();

            StateService.DataSourceState.Subscribe(async (stateTask) =>
            {
                Debug.WriteLine($"DataGridVirtualTablePanel DataSourceState: {dataSourceState}");
                await InvokeAsync(async () => {
                    dataSourceState = await stateTask;
                    StateHasChanged();
                });
            });
            Style = new DataGridStyleInfo
            {
                CssStyle = LayoutService.OuterStyle.Value,
                ActualScrollbarWidth = LayoutService.ActualScrollbarWidth
            };

            //DataSourceStateEv = new EventCallbackFactory().Create<DataSourceState>(this, async (data) => await _dataSourceStateEv(data));
            //GridStateService.DataSourceStateEvList.Add(DataSourceStateEv);

            CollectionViewChangedEv = new EventCallbackFactory().Create<IEnumerable<SmDataGridRowModel<T>>>(this, async (data) => await _collectionViewChangedEv(data));
            RepositoryService.CollectionViewChangedEvList.Add(CollectionViewChangedEv);

            return base.OnInitializedAsync();
        }

        private void SubscribeViewCollectionChange()
        {
            RepositoryService.ViewCollectionObservableTask.Subscribe(async (GetViewCollectionTask) =>
            {
                //dataSourceState = DataSourceState.Loading;
                //StateHasChanged();

                View = await GetViewCollectionTask;
                await InvokeAsync(async () =>
                {
                    if (virtualizeComponent is not null)
                    {
                        //dataSourceState = DataSourceState.Idle;
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
