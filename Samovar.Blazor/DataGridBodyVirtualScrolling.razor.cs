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
        [SmInject]
        public IRepositoryService<TItem>? RepositoryService { get; set; }

        [SmInject]
        public ILayoutService? LayoutService { get; set; }

        [SmInject]
        public IVirtualScrollingNavigationStrategy? VirtualScrollingService { get; set; }

        [SmInject]
        public IConstantService? ConstantService { get; set; }

        protected IEnumerable<SmDataGridRowModel<TItem>>? View { get; set; }

        protected ElementReference GridBodyRef;

        private IDisposable viewCollectionObserverHandler;

        public EventCallback<DataSourceStateEnum> DataSourceStateEv { get; set; }

        public EventCallback<IEnumerable<SmDataGridRowModel<TItem>>> CollectionViewChangedEv { get; set; }

        public string ScrollStyle { get; set; } = "";

        protected Task _dataSourceStateEv(DataSourceStateEnum dataSourceState)
        {
            return Task.CompletedTask;
        }

        protected override Task OnInitializedAsync()
        {
            VirtualScrollingService.VirtualScrollingInfo.Subscribe(VirtualScrollingInfoSubscriber);

            DataSourceStateEv = new EventCallbackFactory().Create<DataSourceStateEnum>(this, async (data) => await _dataSourceStateEv(data));
            CollectionViewChangedEv = new EventCallbackFactory().Create<IEnumerable<SmDataGridRowModel<TItem>>>(this, async (data) => await _collectionViewChangedEv(data));

            RepositoryService.CollectionViewChangedEvList.Add(CollectionViewChangedEv);

            return base.OnInitializedAsync();
        }

        private void VirtualScrollingInfoSubscriber(DataGridVirtualScrollingInfo info)
        {
            ScrollStyle = $"height:{info.ContentContainerHeight.ToString(CultureInfo.InvariantCulture)}px;overflow:hidden;position:absolute;table-layout:fixed;";
            ScrollStyle += LayoutService.MinGridWidth.Value > 0 ? "min-width:"+LayoutService.MinGridWidth.Value.ToString(CultureInfo.InvariantCulture)+"px;" : "";
            ScrollStyle += $"transform:translateY({(-info.OffsetY).ToString(CultureInfo.InvariantCulture)}px);";
        }

        private Task _collectionViewChangedEv(IEnumerable<SmDataGridRowModel<TItem>> collectionView)
        {
            View = collectionView;
            return Task.CompletedTask;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                await LayoutService.CheckScrollBarWidth();
        }

        public ValueTask DisposeAsync()
        {
            viewCollectionObserverHandler?.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
