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
        protected override Task OnInitializedAsync()
        {
            RepositoryService.ViewCollectionObservable.Subscribe(ViewCollectionObserver);
            //RepositoryService.ViewCollectionChanged += RepositoryService_ViewCollectionChanged;

            //if(RepositoryService.ViewCollection != null)
            //    View = RepositoryService.ViewCollection;

            return base.OnInitializedAsync();
        }

        private void ViewCollectionObserver(Task<IEnumerable<SmDataGridRowModel<TItem>>> task)
        {
            if (task.Result == null)
                return;
            View = task.Result;
            StateHasChanged();
        }

        //private void ViewCollectionObserver(IEnumerable<SmDataGridRowModel<TItem>> viewCollection)
        //{
        //    View = viewCollection;
        //    StateHasChanged();
        //}

        protected override async Task OnAfterRenderAsync(bool firstRender)
		{
            if(!firstRender)
                await LayoutService.CheckScrollBarWidth();
		}

		//private async Task RepositoryService_ViewCollectionChanged(IEnumerable<SmDataGridRowModel<TItem>> arg)
  //      {
  //          View = arg;
  //          await InvokeAsync(StateHasChanged);
  //      }

        public ValueTask DisposeAsync()
        {
            //RepositoryService.ViewCollectionChanged -= RepositoryService_ViewCollectionChanged;
            return ValueTask.CompletedTask;
        }
    }
}
