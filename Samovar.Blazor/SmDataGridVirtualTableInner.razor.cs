using Microsoft.AspNetCore.Components;

namespace Samovar.Blazor
{
    public partial class SmDataGridVirtualTableInner<T>
        : SmDesignComponentBase, IAsyncDisposable
    {
        [SmInject]
        public required IGridStateService StateService { get; set; }

        [SmInject]
        public required ILayoutService LayoutService { get; set; }

        [SmInject]
        public required IConstantService ConstantService { get; set; }

        [SmInject]
        public required IRepositoryService<T> RepositoryService { get; set; }

        [SmInject]
        public required INavigationService NavigationService { get; set; }

        [SmInject]
        public required IComponentBuilderService ComponentBuilderService { get; set; }

        [SmInject]
        public required IEditingService<T> EditingService { get; set; }

        public RenderFragment? EditingPopup { get; set; }

        public RenderFragment? InsertingPopup { get; set; }

        public RenderFragment? InsertingForm { get; set; }

        public RenderFragment? NoDataPanel { get; set; }
        public RenderFragment? DataPanel { get; set; }

        public RenderFragment? NoDataFoundPanel { get; set; }
        public RenderFragment? PagingPanel { get; set; }

        public RenderFragment? DataProcessingPanel { get; set; }

        public required DataGridStyleInfo Style { get; set; }

        protected override Task OnInitializedAsync()
        {
            LayoutService.DataGridInnerStyle.Subscribe(async style => {
                Style = await style;
                StateHasChanged();
            });

            //Popup editing
            EditingService.ShowEditingPopupDelegate = (SmDataGridRowModel<T> model) => { EditingPopup = ComponentBuilderService.GetEditingPopup(model); StateHasChanged(); return Task.CompletedTask; };
            EditingService.CloseEditingPopupDelegate = () => { EditingPopup = null; StateHasChanged(); return Task.CompletedTask; };

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

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
