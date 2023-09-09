using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public partial class SmDataGridInner<T>
        : SmDesignComponentBase , IAsyncDisposable
    {
        [SmInject]
        public IGridStateService StateService { get; set; }


        [SmInject]
        public ILayoutService LayoutService { get; set; }

        [SmInject]
        public IConstantService ConstantService { get; set; }

        [SmInject]
        public IVirtualScrollingService VirtualScrollingService { get; set; }

        [SmInject]
        public IRepositoryService<T> RepositoryService { get; set; }

        [SmInject]
        public INavigationService NavigationService { get; set; }
        
        [SmInject]
        public IComponentBuilderService ComponentBuilderService { get; set; }

        [SmInject]
        public IEditingService<T> EditingService { get; set; }

        public RenderFragment EditingPopup { get; set; }

        public RenderFragment InsertingPopup { get; set; }

        public RenderFragment InsertingForm { get; set; }

        public RenderFragment NoDataPanel { get; set; }
        public RenderFragment DataPanel { get; set; }

        public RenderFragment NoDataFoundPanel { get; set; }
        public RenderFragment PagingPanel { get; set; }
        
        public RenderFragment DataProcessingPanel { get; set; }

        public DataGridStyleInfo Style { get; set; } //Default style
        
        protected override Task OnInitializedAsync()
        {
            //var sub1 = new Subscription1TaskVoid<DataGridVirtualScrollingInfo>(VirtualScrollingService.VirtualScrollingInfo, myfunc1);
            //sub1.CreateMap();

            Style = new DataGridStyleInfo
            {
                CssStyle = LayoutService.OuterStyle.SubjectValue,
                ActualScrollbarWidth = LayoutService.ActualScrollbarWidth
            };

            LayoutService.DataGridInnerCssStyleChanged += GridLayoutService_DataGridInnerCssStyleChanged;

            //Popup editing
            EditingService.ShowEditingPopupDelegate = (SmDataGridRowModel<T> model) => { EditingPopup = ComponentBuilderService.GetEditingPopup(model); StateHasChanged(); return Task.CompletedTask;  };
            EditingService.CloseEditingPopupDelegate = () => { EditingPopup = null; StateHasChanged(); return Task.CompletedTask;  };

            //Popup inserting
            EditingService.ShowInsertingPopupDelegate = (SmDataGridRowModel<T> model) => { InsertingPopup = ComponentBuilderService.GetInsertingPopup<T>(model); StateHasChanged(); return Task.CompletedTask; };
            EditingService.CloseInsertingPopupDelegate = () => { InsertingPopup = null; StateHasChanged(); return Task.CompletedTask; };

            //Inline inserting form
            EditingService.ShowInsertingFormDelegate = (SmDataGridRowModel<T> model) => { InsertingForm = ComponentBuilderService.GetInsertingForm(model); StateHasChanged(); return Task.CompletedTask; };
            EditingService.CloseInsertingFormDelegate = () => { InsertingForm = null; StateHasChanged(); return Task.CompletedTask; };

            //Data processing panels

            //Data panel
            StateService.ShowDataPanelDelegate = () => { DataPanel = ComponentBuilderService.GetDataPanel<T>(); StateHasChanged(); return Task.CompletedTask; };
            StateService.CloseDataPanelDelegate = () => { DataPanel = null; StateHasChanged(); return Task.CompletedTask; };

            //No data panel
            StateService.ShowNoDataPanelDelegate = () => { NoDataPanel = ComponentBuilderService.GetNoDataPanel(); StateHasChanged(); return Task.CompletedTask; };
            StateService.CloseNoDataPanelDelegate = () => { NoDataPanel = null; StateHasChanged(); return Task.CompletedTask; };

            //No data found panel
            StateService.ShowNoDataFoundPanelDelegate = () => { NoDataFoundPanel = ComponentBuilderService.GetNoDataFoundPanel(); StateHasChanged(); return Task.CompletedTask; };
            StateService.CloseNoDataFoundPanelDelegate = () => { NoDataFoundPanel = null; StateHasChanged(); return Task.CompletedTask; };

            //Processing data panel
            StateService.ShowProcessingDataPanelDelegate = () => { DataProcessingPanel = ComponentBuilderService.GetProcessingDataPanel(); StateHasChanged(); return Task.CompletedTask; };
            StateService.CloseProcessingDataPanelDelegate = () => { DataProcessingPanel = null; StateHasChanged(); return Task.CompletedTask; };

            //Paging footer
            StateService.ShowPagingPanelDelegate = () => { PagingPanel = ComponentBuilderService.GetPagingPanel<T>(); StateHasChanged(); return Task.CompletedTask; };
            StateService.HidePagingPanelDelegate = () => { PagingPanel = null; StateHasChanged(); return Task.CompletedTask; };


            base.OnInitializedAsync();

            return Task.CompletedTask;
        }

        private async Task GridLayoutService_DataGridInnerCssStyleChanged(DataGridStyleInfo arg)
        {
            Style = arg;
            await InvokeAsync(StateHasChanged);
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
        //public string ScrollStyle { get; set; }
        //public double OffsetY { get; set; }
        //public Task<string> ScrollStyle()
        //{
        //    var height = await VirtualScrollingService.TranslatableDivHeight();
        //    return Task.FromResult( $"height:{height};overflow:hidden;position:absolute;");
        //}

        public ValueTask DisposeAsync()
        {
            LayoutService.DataGridInnerCssStyleChanged -= GridLayoutService_DataGridInnerCssStyleChanged;
            return ValueTask.CompletedTask;
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
