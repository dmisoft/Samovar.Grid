using System;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public partial class DataGridBodyPanel<T>
        : SmDesignComponentBase, IDisposable
    {
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

        public DataGridStyleInfo Style { get; set; } //Default style
        
        public string ScrollStyle { get; set; }
        public double OffsetY { get; set; }

        protected override Task OnInitializedAsync()
        {
            var sub1 = new Subscription1TaskVoid<DataGridVirtualScrollingInfo>(VirtualScrollingService.VirtualScrollingInfo, myfunc1);
            sub1.CreateMap();

            Style = new DataGridStyleInfo
            {
                CssStyle = LayoutService.OuterStyle.SubjectValue,
                ActualScrollbarWidth = LayoutService.ActualScrollbarWidth
            };
            
            LayoutService.DataGridInnerCssStyleChanged += LayoutService_DataGridInnerCssStyleChanged;

            return base.OnInitializedAsync();   
        }
        private Task myfunc1(DataGridVirtualScrollingInfo arg)
        {
            ScrollStyle = $"height:{arg.TranslatableDivHeight};overflow:hidden;position:absolute;";
            OffsetY = arg.OffsetY;
            StateHasChanged();
            return Task.CompletedTask;
            //await InvokeAsync(() => {
            //    ScrollStyle = $"height:{arg.TranslatableDivHeight};overflow:hidden;position:absolute;";
            //    OffsetY = arg.OffsetY;
            //    StateHasChanged();
            //});
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
