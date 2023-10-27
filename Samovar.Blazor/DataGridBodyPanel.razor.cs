using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public partial class DataGridBodyPanel<T>
        : SmDesignComponentBase, IDisposable
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
        public IVirtualScrollingService VirtualScrollingService { get; set; }

		[SmInject]
		public IGridStateService GridStateService { get; set; }

        [Parameter]
        public EventCallback<DataSourceStateEnum> DataSourceStateEv { get; set; }

        public DataGridStyleInfo Style { get; set; } //Default style
        
        public string ScrollStyle { get; set; }
        public double OffsetY { get; set; }

        protected Task _dataSourceStateEv(DataSourceStateEnum dataSourceState) { 
            _dataSourceState = dataSourceState;
            //StateHasChanged();
            return Task.CompletedTask;
        }

        protected override Task OnInitializedAsync()
        {
            
            //TODO refactoring 10/2023
            //var sub1 = new Subscription1TaskVoid<DataGridVirtualScrollingInfo>(VirtualScrollingService.VirtualScrollingInfo, myfunc1);
            //sub1.CreateMap();
            VirtualScrollingService.VirtualScrollingInfo.Subscribe(myfunc1);

            Style = new DataGridStyleInfo
            {
                CssStyle = LayoutService.OuterStyle.Value,
                ActualScrollbarWidth = LayoutService.ActualScrollbarWidth
            };
            
            LayoutService.DataGridInnerCssStyleChanged += LayoutService_DataGridInnerCssStyleChanged;
            //GridStateService.DataSourceState.Subscribe(async dataSourceState => await ProcessDataSourceState(dataSourceState));
            
            DataSourceStateEv = new EventCallbackFactory().Create<DataSourceStateEnum>(this, async (data) => await _dataSourceStateEv(data));

            //GridStateService.DataSourceStateEv = DataSourceStateEv;
            //GridStateService.DataSourceStateEv = _dataSourceStateEv;
            GridStateService.DataSourceStateEvList.Add(DataSourceStateEv);

            return base.OnInitializedAsync();   
        }

  //      private Task ProcessDataSourceState(DataSourceStateEnum dataSourceState)
		//{
  //          _dataSourceState = dataSourceState;
  //          return Task.CompletedTask;
		//}

		private void myfunc1(DataGridVirtualScrollingInfo arg)
        {
            ScrollStyle = $"height:{arg.TranslatableDivHeight};overflow:hidden;position:absolute;";
            OffsetY = arg.OffsetY;
            StateHasChanged();
        }

        private async Task LayoutService_DataGridInnerCssStyleChanged(DataGridStyleInfo arg)
        {
            Style = arg;
            await InvokeAsync(StateHasChanged);
        }
        public void Dispose()
        {
            LayoutService.DataGridInnerCssStyleChanged -= LayoutService_DataGridInnerCssStyleChanged;
        }
    }
}
