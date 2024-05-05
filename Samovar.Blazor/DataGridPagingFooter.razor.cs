using Microsoft.AspNetCore.Components;
using System;

namespace Samovar.Blazor
{
	public partial class DataGridPagingFooter
        : SmDesignComponentBase, IAsyncDisposable, IObserver<DataGridPagerInfo>
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [SmInject]
        public ILayoutService LayoutService { get; set; }

        [SmInject]
        public IPagingNavigationStrategy PagingNavigationStrategy { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

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

        public ValueTask DisposeAsync()
        {
            return new ValueTask();
        }
    }
}
