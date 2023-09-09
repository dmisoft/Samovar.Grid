using Microsoft.AspNetCore.Components;
using System;

namespace Samovar.Blazor
{
	public partial class DataGridPagingFooter<TItem>
        : SmDesignComponentBase, IDisposable, IObserver<DataGridPagerInfo>
    {
        //[SmInject]
        //public IRepositoryService<TItem> RepositoryService { get; set; }

        [SmInject]
        public ILayoutService LayoutService { get; set; }

        [SmInject]
        public IPagingNavigationStrategy PagingNavigationStrategy { get; set; }

        internal ElementReference GridFooterRef { get; set; }

        protected override void OnInitialized()
        {
            PagingNavigationStrategy.PagerInfo.Subscribe(this);
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(DataGridPagerInfo value)
        {
            InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
        }
    }
}
