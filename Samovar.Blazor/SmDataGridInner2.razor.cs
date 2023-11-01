using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public partial class SmDataGridInner2<T>
        : SmDesignComponentBase , IDisposable
    {
        [SmInject]
        public ILayoutService GridLayoutService { get; set; }

        [SmInject]
        public IConstantService ConstantService { get; set; }

        [SmInject]
        public IVirtualScrollingNavigationStrategy VirtualScrollingService { get; set; }

        [SmInject]
        public IRepositoryService<T> RepositoryService { get; set; }

        [SmInject]
        public INavigationService NavigationService { get; set; }
        
        public DataGridStyleInfo Style { get; set; } //Default style
        
        protected override void OnInitialized()
        {
            //TODO refactoring 10/2023
            //var sub1 = new Subscription1TaskVoid<DataGridVirtualScrollingInfo>(VirtualScrollingService.VirtualScrollingInfo, myfunc1);
            //sub1.CreateMap();

            Style = new DataGridStyleInfo { 
                CssStyle = GridLayoutService.OuterStyle.Value,
                ActualScrollbarWidth = GridLayoutService.ActualScrollbarWidth
            };
            GridLayoutService.DataGridInnerCssStyleChanged += GridLayoutService_DataGridInnerCssStyleChanged;
            base.OnInitialized();
        }

        private Task myfunc1(DataGridVirtualScrollingInfo arg)
        {
            ScrollStyle = $"height:{arg.TranslatableDivHeight};overflow:hidden;position:absolute;";
            OffsetY = arg.OffsetY;
            StateHasChanged();
            return Task.CompletedTask;
        }

        private async Task GridLayoutService_DataGridInnerCssStyleChanged(DataGridStyleInfo arg)
        {
            Style = arg;
            await InvokeAsync(StateHasChanged);
        }
        
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);
        }
        //[Parameter]
        //public string FilterToggleButtonClass { get; set; } = "btn btn-secondary";
        //[Parameter]
        //public string PaginationClass { get; set; } = "pagination";

        //internal string GridBodyContainerId { get; } = $"gridbodycontainer{Guid.NewGuid().ToString().Replace("-", "")}";
        //internal string innerGridBodyTableId { get; } = $"innergridbodytable{Guid.NewGuid().ToString().Replace("-", "")}";

        //internal string outerGridId { get; set; } = $"outergrid{Guid.NewGuid().ToString().Replace("-", "")}";
        //internal string gridHeaderContainerId { get; set; } = $"gridheadercontainer{Guid.NewGuid().ToString().Replace("-", "")}";
        //internal string gridFilterContainerId { get; set; } = $"gridfiltercontainer{Guid.NewGuid().ToString().Replace("-", "")}";
        //private string DataGridId { get; } = $"samovargrid{Guid.NewGuid().ToString().Replace("-", "")}";

        //internal double MinGridWidth { get; set; }
        //[SmInject]
        //IColumnService ColumnService { get; set; }
        //public async Task<string> TranslatableDivHeight() {
        //    return await VirtualScrollingService.TranslatableDivHeight();
        //}

        //public async Task<string> ScrollStyle() {
        //    var height = await VirtualScrollingService.TranslatableDivHeight();
        //    return $"height:{height};overflow:hidden;position:absolute;";
        //}
        public string ScrollStyle { get; set; }
        public double OffsetY { get; set; }
        //public Task<string> ScrollStyle()
        //{
        //    var height = await VirtualScrollingService.TranslatableDivHeight();
        //    return Task.FromResult( $"height:{height};overflow:hidden;position:absolute;");
        //}
        public void Dispose()
        {
            GridLayoutService.DataGridInnerCssStyleChanged -= GridLayoutService_DataGridInnerCssStyleChanged;
        }

        //public void OnCompleted()
        //{
        //    throw new NotImplementedException();
        //}

        //public void OnError(Exception error)
        //{
        //    throw new NotImplementedException();
        //}

        //public string ScrollStyle { get; set; }
        //public void OnNext(string height)
        //{
        //    ScrollStyle = $"height:{height};overflow:hidden;position:absolute;";
        //}
    }
}
