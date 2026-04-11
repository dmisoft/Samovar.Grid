using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Reactive.Linq;

namespace Samovar.Grid;

public partial class PagingGrid<T>
    : DesignComponentBase, IAsyncDisposable
{
    public ElementReference GridBodyRef { get; set; }
    protected IEnumerable<GridRowModel<T>> View { get; set; } = [];
    public DataSourceState DataSourceState { get; set; } = DataSourceState.NoData;

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

    [SmInject]
    public required IJsService JsService { get; set; }

    public RenderFragment? EditingPopup { get; set; }

    public RenderFragment? InsertingPopup { get; set; }

    public RenderFragment? InsertingForm { get; set; }

    public RenderFragment? NoDataPanel { get; set; }
    public RenderFragment? DataPanel { get; set; }

    public RenderFragment? NoDataFoundPanel { get; set; }
    public RenderFragment? PagingPanel { get; set; }

    public RenderFragment? DataProcessingPanel { get; set; }

    public required GridStyleInfo Style { get; set; }

    [Parameter]
    public bool ShowCommandBar { get; set; } = true;

    private SmScrollbar? _verticalScrollbar;
    private SmScrollbar? _horizontalScrollbar;
    private bool _scrollbarsInitialized;

    private bool _firstRenderComplete;
    private double _savedScrollLeft;
    private bool _restoreScrollLeft;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            _firstRenderComplete = true;

        if (!_scrollbarsInitialized && _verticalScrollbar is not null && _horizontalScrollbar is not null)
        {
            _scrollbarsInitialized = true;
            await LayoutService.GridInnerRef.InitCustomScrollbars(
                await JsService.JsModule(),
                _verticalScrollbar.TrackRef,
                _horizontalScrollbar.TrackRef,
                _verticalScrollbar.ThumbRef,
                _horizontalScrollbar.ThumbRef);
        }

        if (_restoreScrollLeft)
        {
            _restoreScrollLeft = false;
            await LayoutService.GridInnerRef
                .SetElementScrollLeft(await JsService.JsModule(), _savedScrollLeft);
        }
    }

    protected string CssClass = "";
    protected string _tableSizeClass = "";

    protected override Task OnInitializedAsync()
    {
        SubscribeViewCollectionChange();

        LayoutService.CssClass.Subscribe(_ => { CssClass = _; });
        LayoutService.SizeMode.Subscribe(mode =>
        {
            _tableSizeClass = mode switch { GridSizeMode.Small => "table-sm small", GridSizeMode.Large => "sm-table-lg", _ => "" };
            StateHasChanged();
        });

        StateService.DataSourceState.Subscribe(async (stateTask) =>
        {
            await InvokeAsync(async () =>
            {
                DataSourceState = await stateTask;
                StateHasChanged();
            });
        });
        LayoutService.DataGridInnerStyle.Subscribe(async style =>
        {
            Style = await style;
            StateHasChanged();
        });

        //Popup editing
        EditingService.ShowEditingPopupDelegate = (GridRowModel<T> model) => { EditingPopup = ComponentBuilderService.GetEditingPopup(model); StateHasChanged(); return Task.CompletedTask; };
        EditingService.CloseEditingPopupDelegate = () => { EditingPopup = null; StateHasChanged(); return Task.CompletedTask; };

        //Popup inserting
        EditingService.ShowInsertingPopupDelegate = (GridRowModel<T> model) => { InsertingPopup = ComponentBuilderService.GetInsertingPopup<T>(model); StateHasChanged(); return Task.CompletedTask; };
        EditingService.CloseInsertingPopupDelegate = () => { InsertingPopup = null; StateHasChanged(); return Task.CompletedTask; };

        //Inline inserting form
        EditingService.ShowInsertingFormDelegate = (GridRowModel<T> model) => { InsertingForm = ComponentBuilderService.GetInsertingForm(model); StateHasChanged(); return Task.CompletedTask; };
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

    private void SubscribeViewCollectionChange()
    {
        RepositoryService.ViewCollectionObservableTask
            .DistinctUntilChanged()
            .Subscribe(async (GetViewCollectionTask) =>
        {
            if (_firstRenderComplete)
            {
                _savedScrollLeft = await LayoutService.GridInnerRef
                    .GetElementScrollLeft(await JsService.JsModule());
                _restoreScrollLeft = _savedScrollLeft > 0;
            }
            View = await GetViewCollectionTask;
            await InvokeAsync(() => StateHasChanged());
        });
    }

    public async ValueTask DisposeAsync()
    {
        if (_scrollbarsInitialized)
        {
            try
            {
                await LayoutService.GridInnerRef.DisposeCustomScrollbars(await JsService.JsModule());
            }
            catch (JSDisconnectedException) { }
        }
    }
}
