using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public partial class DataGridBodyPanel<T>
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

        //[Parameter]
        public EventCallback<DataSourceStateEnum> DataSourceStateEv { get; set; }

        public DataGridStyleInfo Style { get; set; } //Default style
        
        //public string ScrollStyle { get; set; }
        //public double OffsetY { get; set; }

        protected Task _dataSourceStateEv(DataSourceStateEnum dataSourceState) { 

            _dataSourceState = dataSourceState;
            //StateHasChanged();
            return Task.CompletedTask;
        }

        protected override Task OnInitializedAsync()
        {
            //VirtualScrollingService.VirtualScrollingInfo.Subscribe(myfunc1);

            Style = new DataGridStyleInfo
            {
                CssStyle = LayoutService.OuterStyle.Value,
                ActualScrollbarWidth = LayoutService.ActualScrollbarWidth
            };
            
            //LayoutService.DataGridInnerCssStyleChanged += LayoutService_DataGridInnerCssStyleChanged;
            DataSourceStateEv = new EventCallbackFactory().Create<DataSourceStateEnum>(this, async (data) => await _dataSourceStateEv(data));
            GridStateService.DataSourceStateEvList.Add(DataSourceStateEv);

            return base.OnInitializedAsync();   
        }

		private void myfunc1(DataGridVirtualScrollingInfo arg)
        {
            //ScrollStyle = $"height:{arg.TranslatableDivHeight};overflow:hidden;position:absolute;";
            //OffsetY = arg.OffsetY;
            //StateHasChanged();
        }

        //private async Task LayoutService_DataGridInnerCssStyleChanged(DataGridStyleInfo arg)
        //{
        //    Style = arg;
        //    await InvokeAsync(StateHasChanged);
        //}

        public ValueTask DisposeAsync()
        {
            GridStateService.DataSourceStateEvList.Remove(DataSourceStateEv);
            //LayoutService.DataGridInnerCssStyleChanged -= LayoutService_DataGridInnerCssStyleChanged;
            return ValueTask.CompletedTask;
        }
    }
}
