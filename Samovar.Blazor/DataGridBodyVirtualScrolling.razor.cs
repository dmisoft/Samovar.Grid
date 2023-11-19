﻿using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public partial class DataGridBodyVirtualScrolling<TItem>
        : SmDesignComponentBase, IAsyncDisposable
    {
        //protected DataSourceStateEnum _dataSourceState = DataSourceStateEnum.NoData;

        [SmInject]
        public IRepositoryService<TItem> RepositoryService { get; set; }

        [SmInject]
        public ILayoutService LayoutService { get; set; }

        [SmInject]
        public IVirtualScrollingNavigationStrategy VirtualScrollingService { get; set; }


        //[SmInject]
        //public IEditingService<TItem> EditingService { get; set; }

        //[SmInject]
        //public IComponentBuilderService ComponentBuilderService { get; set; }

        [SmInject]
        public IConstantService ConstantService { get; set; }

        //[SmInject]
        //public IGridStateService GridStateService { get; set; }

        //[SmInject]
        //public INavigationService NavigationService { get; set; }

        //[Parameter]
        //public Action<IEnumerable<SmDataGridRowModel<TItem>>> CollectionViewChanged { get; set; }

        protected IEnumerable<SmDataGridRowModel<TItem>> View { get; set; }

        public ElementReference GridBodyRef;

        private IDisposable viewCollectionObserverHandler;

        //[Parameter]
        public EventCallback<DataSourceStateEnum> DataSourceStateEv { get; set; }

        //[Parameter]
        public EventCallback<IEnumerable<SmDataGridRowModel<TItem>>> CollectionViewChangedEv { get; set; }

        public string ScrollStyle { get; set; }
        //public double OffsetY { get; set; }

        protected Task _dataSourceStateEv(DataSourceStateEnum dataSourceState)
        {
            return Task.CompletedTask;
        }

        protected override Task OnInitializedAsync()
        {
            VirtualScrollingService.VirtualScrollingInfo.Subscribe(VirtualScrollingInfoSubscriber);

            DataSourceStateEv = new EventCallbackFactory().Create<DataSourceStateEnum>(this, async (data) => await _dataSourceStateEv(data));
            CollectionViewChangedEv = new EventCallbackFactory().Create<IEnumerable<SmDataGridRowModel<TItem>>>(this, async (data) => await _collectionViewChangedEv(data));

            //GridStateService.DataSourceStateEvList.Add(DataSourceStateEv);
            RepositoryService.CollectionViewChangedEvList.Add(CollectionViewChangedEv);

            return base.OnInitializedAsync();
        }

        private void VirtualScrollingInfoSubscriber(DataGridVirtualScrollingInfo info)
        {
            //OffsetY = info.OffsetY;
            //$"{_translatableDivHeight.ToString(CultureInfo.InvariantCulture)}px"

            ScrollStyle = $"height:{info.ContentContainerHeight.ToString(CultureInfo.InvariantCulture)}px;overflow:hidden;position:absolute;table-layout:fixed;";
            ScrollStyle += LayoutService.MinGridWidth.Value > 0 ? "min-width:"+LayoutService.MinGridWidth.Value.ToString(CultureInfo.InvariantCulture)+"px;" : "";
            ScrollStyle += $"transform:translateY({(-info.OffsetY).ToString(CultureInfo.InvariantCulture)}px);";

            //TODO ry to avoid StateHasChanged 
            //StateHasChanged();
        }

        private Task foo(EventCallback<DataSourceStateEnum> callback)
        {
            callback.InvokeAsync();
            return Task.CompletedTask;
        }

        private Task _collectionViewChangedEv(IEnumerable<SmDataGridRowModel<TItem>> collectionView)
        {
            View = collectionView;
            return Task.CompletedTask;
        }

        //private Task ViewCollectionObserver(IEnumerable<SmDataGridRowModel<TItem>> collection)
        //{
        //    View = collection;
        //    //StateHasChanged();
        //    return Task.CompletedTask;
        //}

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                await LayoutService.CheckScrollBarWidth();
        }

        public ValueTask DisposeAsync()
        {
            viewCollectionObserverHandler?.Dispose();
            //GridStateService.DataSourceStateEvList.Remove(DataSourceStateEv);
            //RepositoryService.CollectionViewChangedEvList.Remove(CollectionViewChangedEv);
            return ValueTask.CompletedTask;
        }
    }
}