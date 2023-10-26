using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public partial class DataGridBody<TItem>
        : SmDesignComponentBase, IAsyncDisposable
    {
        [SmInject]
        public IRepositoryService<TItem> RepositoryService { get; set; }

        [SmInject]
        public ILayoutService LayoutService { get; set; }

        [SmInject]
        public IEditingService<TItem> EditingService { get; set; }

        [SmInject]
        public IComponentBuilderService ComponentBuilderService { get; set; }

        [SmInject]
        public IConstantService ConstantService { get; set; }


        protected IEnumerable<SmDataGridRowModel<TItem>> View { get; set; }

        public ElementReference GridBodyRef;

        private IDisposable viewCollectionObserverHandler;

        protected override Task OnInitializedAsync()
        {
            viewCollectionObserverHandler = RepositoryService.ViewCollectionObservable.Subscribe(async (viewCollection)=> await ViewCollectionObserver(viewCollection));
            return base.OnInitializedAsync();
        }

        private Task ViewCollectionObserver(IEnumerable<SmDataGridRowModel<TItem>> collection)
        {
            View = collection;
            StateHasChanged();
            return Task.CompletedTask;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
		{
            if(!firstRender)
                await LayoutService.CheckScrollBarWidth();
		}

        public ValueTask DisposeAsync()
        {
            viewCollectionObserverHandler.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
