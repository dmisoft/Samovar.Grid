using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Samovar.Grid;

public class LayoutService
    : ILayoutService, IAsyncDisposable
{
    private const string BaseTableCssClass = "table";
    private const string BasePaginationCssClass = "pagination";

    public BehaviorSubject<GridColumnResizeMode> ColumnResizeMode { get; } = new BehaviorSubject<GridColumnResizeMode>(GridColumnResizeMode.None);
    public BehaviorSubject<GridSizeMode> SizeMode { get; } = new BehaviorSubject<GridSizeMode>(GridSizeMode.Default);
    public BehaviorSubject<string> CssClass { get; } = new BehaviorSubject<string>(BaseTableCssClass);
    public BehaviorSubject<string> PaginationCssClass { get; } = new BehaviorSubject<string>(BasePaginationCssClass);
    public BehaviorSubject<double> MinGridWidth { get; } = new BehaviorSubject<double>(0d);
    public BehaviorSubject<bool> ShowDetailRow { get; } = new BehaviorSubject<bool>(false);
    public BehaviorSubject<bool> ShowRowSelectionColumn { get; } = new BehaviorSubject<bool>(false);
    public BehaviorSubject<GridFilterMode> FilterMode { get; } = new BehaviorSubject<GridFilterMode>(GridFilterMode.None);
    public ElementReference GridFilterRef { get; set; }
    public ElementReference GridOuterRef { get; set; }
    public ElementReference GridInnerRef { get; set; }
    public ElementReference TableBodyInnerRef { get; set; }

    public double ActualColumnsWidthSum
    {
        get
        {
            return _columnService.AllColumnModels.Sum(c => c.Width.Value) +
                    (ShowDetailRow.Value ? _columnService.DetailExpanderColumnModel.Width.Value : 0d) +
                    (ShowRowSelectionColumn.Value ? _columnService.RowSelectionColumnModel.Width.Value : 0d);
        }
    }

    private readonly IConstantService _constantService;
    private readonly IJsService _jsService;
    private readonly IColumnService _columnService;

    public DotNetObjectReference<ILayoutService> DataGridDotNetRef { get; }

    public LayoutService(
          IConstantService constantService
        , IJsService jsService
        , IInitService initService
        , IColumnService columnService)
    {
        _constantService = constantService;
        _jsService = jsService;
        _columnService = columnService;

        initService.IsInitialized.Subscribe(DataGridInitializerCallback);

        DataGridDotNetRef = DotNetObjectReference.Create(this as ILayoutService);
        DataGridInnerStyle = Observable.CombineLatest(Width, Height)
            .Select(_ =>
            {
                string outerStyle = $"height:{_[1]};";
                if (!string.IsNullOrEmpty(_[0]))
                {
                    outerStyle += $"width:{_[0]};";
                }

                return Task.FromResult(new GridStyleInfo { CssStyle = outerStyle });
            });
    }

    private void DataGridInitializerCallback(bool obj)
    {
        Task.Run(async () => await HeightWidthChanged(height: Height.Value, width: Width.Value));
    }

    public event Func<GridStyleInfo, Task>? DataGridInnerCssStyleChanged = null;

    public BehaviorSubject<string> OuterStyle { get; } = new BehaviorSubject<string>("");

    public BehaviorSubject<string> FooterStyle { get; } = new BehaviorSubject<string>("");

    public BehaviorSubject<string> Height { get; } = new BehaviorSubject<string>("400px");

    public BehaviorSubject<string> Width { get; } = new BehaviorSubject<string>("1200px");

    public BehaviorSubject<bool> ShowColumnHeader { get; } = new BehaviorSubject<bool>(true);

    public IObservable<Task<GridStyleInfo>> DataGridInnerStyle { get; }
    public bool ColumnsWidthTouchedByUser { get; set; } = false;

    private async Task HeightWidthChanged(string height, string width)
    {
        string outerStyle = $"height:{height};";
        string footerStyle = "";
        if (!string.IsNullOrEmpty(width))
        {
            outerStyle += $"width:{width};";
            footerStyle = $"width:{width};";
        }

        OuterStyle.OnNext(outerStyle);
        FooterStyle.OnNext(footerStyle);
        await OnDataGridInnerCssStyleChanged();
    }

    [JSInvokable]
    public async Task JS_AfterWindowResize()
    {
        if(!ColumnsWidthTouchedByUser)
            await CaculateHeader();
    }

    public async Task InitHeader()
    {
        await GridInnerRef.SynchronizeGridHeaderScroll(await _jsService.JsModule(), _constantService.GridHeaderContainerId);
        if (FilterMode.Value == GridFilterMode.FilterRow)
            await GridInnerRef.SynchronizeGridHeaderScroll(await _jsService.JsModule(), _constantService.GridFilterContainerId);

        await CaculateHeader();
    }

    private async Task CaculateHeader()
    {
        double gridInnerWidth = await GridInnerRef.GetElementWidthByRef(await _jsService.JsModule()) - 1;
        var tBodyWidth = await GridOuterRef.GetElementWidthByRef(await _jsService.JsModule());

        // Sum absolute columns (clamped to MinWidth)
        var absoluteColumns = _columnService.DeclarativeColumnModels
            .Where(cmt => cmt.DeclaratedWidthMode == DeclarativeColumnWidthMode.Absolute)
            .ToList();
        var relativeColumns = _columnService.DeclarativeColumnModels
            .Where(cmt => cmt.DeclaratedWidthMode == DeclarativeColumnWidthMode.Relative)
            .ToList();

        var absoluteSum = absoluteColumns.Sum(cmt => Math.Max(cmt.DeclaratedWidth, cmt.MinWidth))
            + (ShowDetailRow.Value ? _columnService.DetailExpanderColumnModel.DeclaratedWidth : 0d)
            + (ShowRowSelectionColumn.Value ? _columnService.RowSelectionColumnModel.DeclaratedWidth : 0d);

        var relativePortionSum = relativeColumns.Sum(cmt => cmt.DeclaratedWidth);

        var widthList = new Dictionary<IColumnModel, double>();

        if (relativePortionSum > 0)
        {
            // Edge Case A: sum of all min-widths exceeds container
            var totalMinWidthSum = _columnService.DeclarativeColumnModels.Sum(cmt => cmt.MinWidth)
                + (ShowDetailRow.Value ? _columnService.DetailExpanderColumnModel.MinWidth : 0d)
                + (ShowRowSelectionColumn.Value ? _columnService.RowSelectionColumnModel.MinWidth : 0d);

            if (totalMinWidthSum > gridInnerWidth)
            {
                // All relative columns get their MinWidth
                foreach (var m in relativeColumns)
                    widthList.Add(m, m.MinWidth);

                foreach (var m in absoluteColumns)
                    widthList.Add(m, Math.Max(m.DeclaratedWidth, m.MinWidth));

                MinGridWidth.OnNext(totalMinWidthSum);
            }
            else
            {
                // Distribute available space to relative columns
                var availableForRelative = gridInnerWidth - absoluteSum;
                var portionValue = availableForRelative / relativePortionSum;

                foreach (var m in relativeColumns)
                    widthList.Add(m, Math.Max(portionValue * m.DeclaratedWidth, m.MinWidth));

                foreach (var m in absoluteColumns)
                    widthList.Add(m, Math.Max(m.DeclaratedWidth, m.MinWidth));

                MinGridWidth.OnNext(0);
            }
        }
        else
        {
            // All columns are absolute
            foreach (var m in absoluteColumns)
                widthList.Add(m, Math.Max(m.DeclaratedWidth, m.MinWidth));

            if (absoluteSum > gridInnerWidth)
                MinGridWidth.OnNext(absoluteSum);
            else
                MinGridWidth.OnNext(0);
        }

        // Calculate empty column width
        var actualSum = widthList.Values.Sum()
            + (ShowDetailRow.Value ? _columnService.DetailExpanderColumnModel.DeclaratedWidth : 0d)
            + (ShowRowSelectionColumn.Value ? _columnService.RowSelectionColumnModel.DeclaratedWidth : 0d);
        var emptyColWidth = Math.Max(tBodyWidth - actualSum, 0);

        _columnService.EmptyColumnModel.Width.OnNext(emptyColWidth);

        // Push widths
        foreach (var m in _columnService.AllColumnModels)
        {
            m.Width.OnNext(widthList[m]);
        }

        if (ShowDetailRow.Value)
        {
            _columnService.DetailExpanderColumnModel.Width.OnNext(_columnService.DetailExpanderColumnModel.DeclaratedWidth);
        }

        if (ShowRowSelectionColumn.Value)
        {
            _columnService.RowSelectionColumnModel.Width.OnNext(_columnService.RowSelectionColumnModel.DeclaratedWidth);
        }
    }

    internal async Task OnDataGridInnerCssStyleChanged()
    {
        if (DataGridInnerCssStyleChanged != null)
        {
            GridStyleInfo info = new GridStyleInfo
            {
                CssStyle = OuterStyle.Value,
            };
            await DataGridInnerCssStyleChanged.Invoke(info);
        }
    }

    public void SetCssClass(string? cssClass)
    {
        cssClass = cssClass?.Trim() ?? "";
        var result = string.IsNullOrWhiteSpace(cssClass)
            ? BaseTableCssClass
            : $"{BaseTableCssClass} {cssClass}";
        CssClass.OnNext(result);
    }

    public void SetPaginationCssClass(string? paginationCssClass)
    {
        paginationCssClass = paginationCssClass?.Trim() ?? "";
        var result = string.IsNullOrWhiteSpace(paginationCssClass)
            ? BasePaginationCssClass
            : $"{BasePaginationCssClass} {paginationCssClass}";
        PaginationCssClass.OnNext(result);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
