using Microsoft.AspNetCore.Components;
using System.Reactive.Linq;

namespace Samovar.Grid.Filter;

public abstract partial class GridFilterCellBase<TFilterCell>
: DesignComponentBase, IAsyncDisposable
{

    [SmInject]
    public required IJsService JsService { get; set; }

    [SmInject]
    public required ILayoutService LayoutService { get; set; }

    [SmInject]
    public required IFilterService FilterService { get; set; }

    [SmInject]
    public required IColumnService ColumnService { get; set; }

    [Parameter]
    public required IDataColumnModel ColMetadata { get; set; }

    protected byte _menuMode { get; set; }

    protected GridFilterCellInfo? FilterCellInfo;

    private TFilterCell? _innerValue = default;

    protected TFilterCell? InnerValue
    {
        set
        {
            _innerValue = value;
            FilterCellInfo = new GridFilterCellInfo { ColumnModel = ColMetadata, FilterCellValue = _innerValue, FilterCellMode = _menuMode };
            FilterService.AddOrRemoveFilter(FilterCellInfo);
        }
        get
        {
            return _innerValue;
        }
    }
    protected string WidthStyle = "";
    protected string _inputSizeClass = "";
    protected string _selectSizeClass = "";

    protected override Task OnInitializedAsync()
    {
        _innerValue = FilterService.TryGetFilterCellValue<TFilterCell>(ColMetadata);
        FilterService.FilterCleared += FilterService_FilterCleared;

        ColMetadata.WidthStyle.Subscribe(w =>
        {
            WidthStyle = w;
            StateHasChanged();
        });
        LayoutService.SizeMode.Subscribe(mode => {
            _inputSizeClass = mode switch { GridSizeMode.Small => "form-control-sm", GridSizeMode.Large => "form-control-lg", _ => "" };
            _selectSizeClass = mode switch { GridSizeMode.Small => "form-select-sm", GridSizeMode.Large => "form-select-lg", _ => "" };
            StateHasChanged();
        });
        return base.OnInitializedAsync();
    }

    protected virtual Task FilterService_FilterCleared()
    {
        _innerValue = default;
        return Task.CompletedTask;
    }

    bool filterMenuOpen = false;

    protected Task ShowMenu()
    {
        filterMenuOpen = !filterMenuOpen;
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        FilterService.FilterCleared -= FilterService_FilterCleared;
        return ValueTask.CompletedTask;
    }
}
