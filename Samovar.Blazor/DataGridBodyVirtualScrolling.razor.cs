using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using System.Diagnostics;
using System.Globalization;

namespace Samovar.Blazor
{
    public partial class DataGridBodyVirtualScrolling<TItem>
        : SmDesignComponentBase, IAsyncDisposable
    {
        [SmInject]
        public required IRepositoryService<TItem> RepositoryService { get; set; }

        [SmInject]
        public required ILayoutService LayoutService { get; set; }

        [SmInject]
        public required IVirtualScrollingNavigationStrategy VirtualScrollingService { get; set; }

        [SmInject]
        public required IConstantService ConstantService { get; set; }

        protected IEnumerable<SmDataGridRowModel<TItem>>? View { get; set; }

        protected ElementReference GridBodyRef;

        public EventCallback<DataSourceState> DataSourceStateEv { get; set; }

        public EventCallback<IEnumerable<SmDataGridRowModel<TItem>>> CollectionViewChangedEv { get; set; }

        public string ScrollStyle { get; set; } = "";

        protected void _dataSourceStateEv(DataSourceState dataSourceState)
        {
            
        }

        private Virtualize<SmDataGridRowModel<TItem>>? virtualizeComponent;
        public DataSourceState dataSourceState = DataSourceState.NoData;

        protected override Task OnInitializedAsync()
        {
            VirtualScrollingService.VirtualScrollingInfo.Subscribe(VirtualScrollingInfoSubscriber);
            //DataSourceStateEv = new EventCallbackFactory().Create<DataSourceState>(this, _dataSourceStateEv);
            //CollectionViewChangedEv = new EventCallbackFactory().Create<IEnumerable<SmDataGridRowModel<TItem>>>(this, _collectionViewChangedEv);
            //RepositoryService.CollectionViewChangedEvList.Add(CollectionViewChangedEv);
            SubscribeViewCollectionChange();

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

        private void VirtualScrollingInfoSubscriber(DataGridVirtualScrollingInfo info)
        {
            ScrollStyle = $"height:{info.ContentContainerHeight.ToString(CultureInfo.InvariantCulture)}px;overflow:hidden;position:absolute;table-layout:fixed;";
            ScrollStyle += LayoutService.MinGridWidth.Value > 0 ? "min-width:" + LayoutService.MinGridWidth.Value.ToString(CultureInfo.InvariantCulture) + "px;" : "";
            ScrollStyle += $"transform:translateY({(-info.OffsetY).ToString(CultureInfo.InvariantCulture)}px);";
            Debug.WriteLine($"ScrollStyle: {ScrollStyle}");
        }

        private Task _collectionViewChangedEv(IEnumerable<SmDataGridRowModel<TItem>> collectionView)
        {
            //View = collectionView;
            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
