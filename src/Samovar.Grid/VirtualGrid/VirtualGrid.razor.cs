using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;

namespace Samovar.Grid;

public partial class VirtualGrid<T>
    : DesignComponentBase, IAsyncDisposable
{
    [SmInject]
    public required IRepositoryService<T> RepositoryService { get; set; }

    [SmInject]
    public required IGridStateService StateService { get; set; }

    [SmInject]
    public required ILayoutService LayoutService { get; set; }

    [SmInject]
    public required IConstantService ConstantService { get; set; }

    [SmInject]
    public required IComponentBuilderService ComponentBuilderService { get; set; }

    [SmInject]
    public required IEditingService<T> EditingService { get; set; }

    [SmInject]
    public required IJsService JsService { get; set; }

    [SmInject]
    public required IGroupingService<T> GroupingService { get; set; }

    protected IEnumerable<GridViewRow<T>> GroupedView { get; set; } = [];
    protected bool IsGroupingActive { get; set; }

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

    public DataSourceState DataSourceState { get; set; } = DataSourceState.NoData;
    public ElementReference GridBodyRef { get; set; }
    protected IEnumerable<GridRowModel<T>> View { get; set; } = [];
    private Virtualize<GridRowModel<T>>? virtualizeComponent;
    protected string CssClass = "";
    protected string _tableSizeClass = "";

    private SmScrollbar? _verticalScrollbar;
    private SmScrollbar? _horizontalScrollbar;
    private bool _scrollbarsInitialized;

    private bool _firstRenderComplete;
    private double _savedScrollLeft;
    private bool _restoreScrollLeft;

    private IDisposable? _cultureSubscription;

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

    protected override Task OnInitializedAsync()
    {
        SubscribeViewCollectionChange();

        GroupingService.IsGroupingActive.Subscribe(active =>
        {
            IsGroupingActive = active;
            _ = InvokeAsync(StateHasChanged);
        });
        GroupingService.GroupedView.Subscribe(rows =>
        {
            GroupedView = rows;
            _ = InvokeAsync(StateHasChanged);
        });

        LayoutService.CssClass.Subscribe(_ => { CssClass = _; });
        LayoutService.SizeMode.Subscribe(mode =>
        {
            _tableSizeClass = mode switch { GridSizeMode.Small => "table-sm small", GridSizeMode.Large => "sm-table-lg", _ => "" };
            _ = InvokeAsync(StateHasChanged);
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
            await InvokeAsync(async () =>
            {
                Style = await style;
                StateHasChanged();
            });
        });

        //Popup editing
        EditingService.ShowEditingPopupDelegate = async (GridRowModel<T> model) => { await InvokeAsync(() => { EditingPopup = ComponentBuilderService.GetEditingPopup(model); StateHasChanged(); }); };
        EditingService.CloseEditingPopupDelegate = async () => { await InvokeAsync(() => { EditingPopup = null; StateHasChanged(); }); };

        //Popup inserting
        EditingService.ShowInsertingPopupDelegate = async (GridRowModel<T> model) => { await InvokeAsync(() => { InsertingPopup = ComponentBuilderService.GetInsertingPopup<T>(model); StateHasChanged(); }); };
        EditingService.CloseInsertingPopupDelegate = async () => { await InvokeAsync(() => { InsertingPopup = null; StateHasChanged(); }); };

        //Inline inserting form
        EditingService.ShowInsertingFormDelegate = async (GridRowModel<T> model) => { await InvokeAsync(() => { InsertingForm = ComponentBuilderService.GetInsertingForm(model); StateHasChanged(); }); };
        EditingService.CloseInsertingFormDelegate = async () => { await InvokeAsync(() => { InsertingForm = null; StateHasChanged(); }); };

        //Data processing panels

        //Data panel
        StateService.ShowDataPanelDelegate = async () => { await InvokeAsync(() => { DataPanel = ComponentBuilderService.GetDataPanel<T>(); StateHasChanged(); }); };
        StateService.CloseDataPanelDelegate = async () => { await InvokeAsync(() => { DataPanel = null; StateHasChanged(); }); };

        //No data panel
        StateService.ShowNoDataPanelDelegate = async () => { await InvokeAsync(() => { NoDataPanel = ComponentBuilderService.GetNoDataPanel(); StateHasChanged(); }); };
        StateService.CloseNoDataPanelDelegate = async () => { await InvokeAsync(() => { NoDataPanel = null; StateHasChanged(); }); };

        //No data found panel
        StateService.ShowNoDataFoundPanelDelegate = async () => { await InvokeAsync(() => { NoDataFoundPanel = ComponentBuilderService.GetNoDataFoundPanel(); StateHasChanged(); }); };
        StateService.CloseNoDataFoundPanelDelegate = async () => { await InvokeAsync(() => { NoDataFoundPanel = null; StateHasChanged(); }); };

        //Processing data panel
        StateService.ShowProcessingDataPanelDelegate = async () => { await InvokeAsync(() => { DataProcessingPanel = ComponentBuilderService.GetProcessingDataPanel(); StateHasChanged(); }); };
        StateService.CloseProcessingDataPanelDelegate = async () => { await InvokeAsync(() => { DataProcessingPanel = null; StateHasChanged(); }); };

        //Paging footer
        StateService.ShowPagingPanelDelegate = async () => { await InvokeAsync(() => { PagingPanel = ComponentBuilderService.GetPagingPanel<T>(); StateHasChanged(); }); };
        StateService.HidePagingPanelDelegate = async () => { await InvokeAsync(() => { PagingPanel = null; StateHasChanged(); }); };

        _cultureSubscription = L10n.CultureChanged.Subscribe(u => InvokeAsync(StateHasChanged));

        base.OnInitializedAsync();

        return Task.CompletedTask;
    }

    private void SubscribeViewCollectionChange()
    {
        RepositoryService.ViewCollectionObservableTask.Subscribe(async (GetViewCollectionTask) =>
        {
            if (_firstRenderComplete)
            {
                _savedScrollLeft = await LayoutService.GridInnerRef
                    .GetElementScrollLeft(await JsService.JsModule());
                _restoreScrollLeft = _savedScrollLeft > 0;
            }
            View = await GetViewCollectionTask;
            await InvokeAsync(async () =>
            {
                if (virtualizeComponent is not null)
                {
                    await virtualizeComponent.RefreshDataAsync();
                    StateHasChanged();
                }
            });
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
        _cultureSubscription?.Dispose();
        _cultureSubscription = null;
    }
}
