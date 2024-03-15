using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public partial class DataGridPagingTablePanel<T>
        : SmDesignComponentBase, IAsyncDisposable
    {
		protected DataSourceStateEnum _dataSourceState = DataSourceStateEnum.NoData;

		[SmInject]
        public IRepositoryService<T> RepositoryService { get; set; }

        [SmInject]
        public ILayoutService LayoutService { get; set; }

        [SmInject]
        public IEditingService<T> EditingService { get; set; }

        [SmInject]
        public IComponentBuilderService ComponentBuilderService { get; set; }

        [SmInject]
        public IConstantService ConstantService { get; set; }

        [SmInject]
        public INavigationService NavigationService { get; set; }

        [SmInject]
        public IVirtualScrollingNavigationStrategy VirtualScrollingService { get; set; }

		[SmInject]
		public IGridStateService GridStateService { get; set; }

        public EventCallback<DataSourceStateEnum> DataSourceStateEv { get; set; }

        public DataGridStyleInfo Style { get; set; } //Default style
        
        public ElementReference GridBodyRef;

        protected IEnumerable<SmDataGridRowModel<T>> View { get; set; }

        private Task _collectionViewChangedEv(IEnumerable<SmDataGridRowModel<T>> collectionView)
        {
            View = collectionView;
            return Task.CompletedTask;
        }


        protected Task _dataSourceStateEv(DataSourceStateEnum dataSourceState) { 
            _dataSourceState = dataSourceState;
            return Task.CompletedTask;
        }
        public EventCallback<IEnumerable<SmDataGridRowModel<T>>> CollectionViewChangedEv { get; set; }

        protected override Task OnInitializedAsync()
        {
            Style = new DataGridStyleInfo
            {
                CssStyle = LayoutService.OuterStyle.Value,
                ActualScrollbarWidth = LayoutService.ActualScrollbarWidth
            };
            
            DataSourceStateEv = new EventCallbackFactory().Create<DataSourceStateEnum>(this, async (data) => await _dataSourceStateEv(data));
            GridStateService.DataSourceStateEvList.Add(DataSourceStateEv);

            CollectionViewChangedEv = new EventCallbackFactory().Create<IEnumerable<SmDataGridRowModel<T>>>(this, async (data) => await _collectionViewChangedEv(data));
            RepositoryService.CollectionViewChangedEvList.Add(CollectionViewChangedEv);

            return base.OnInitializedAsync();   
        }
        

        public ValueTask DisposeAsync()
        {
            GridStateService.DataSourceStateEvList.Remove(DataSourceStateEv);
            RepositoryService.CollectionViewChangedEvList.Remove(CollectionViewChangedEv);
            return ValueTask.CompletedTask;
        }
    }
}
