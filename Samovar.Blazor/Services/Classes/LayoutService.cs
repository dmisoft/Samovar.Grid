using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;

namespace Samovar.Blazor
{
    public class LayoutService
        : ILayoutService, IAsyncDisposable
    {
        public async Task Test()
        {
            var gridInnerWidth = await GridInnerRef.GetElementWidthByRef(await _jsService.JsModule());
            Debug.WriteLine($"GridInnerWidth: {gridInnerWidth}");
        }
        public double MinColumnWidth { get; } = 50d;

        public BehaviorSubject<string> SelectedRowClass { get; } = new BehaviorSubject<string>("bg-warning");

        public BehaviorSubject<string> TableTagClass { get; } = new BehaviorSubject<string>("table table-bordered");

        public BehaviorSubject<string> TheadTagClass { get; } = new BehaviorSubject<string>("table-light");

        public BehaviorSubject<double> MinGridWidth { get; } = new BehaviorSubject<double>(0d);

        public BehaviorSubject<bool> ShowDetailRow { get; } = new BehaviorSubject<bool>(false);

        public BehaviorSubject<string> PaginationClass { get; } = new BehaviorSubject<string>("pagination");
        public BehaviorSubject<string> FilterToggleButtonClass { get; } = new BehaviorSubject<string>("btn btn-secondary");

        public BehaviorSubject<bool> ShowFilterRow { get; } = new BehaviorSubject<bool>(false);

        public BehaviorSubject<DataGridFilterMode> FilterMode { get; } = new BehaviorSubject<DataGridFilterMode>(DataGridFilterMode.None);

        public ElementReference GridFilterRef { get; set; }
        public ElementReference GridOuterRef { get; set; }
        public ElementReference GridInnerRef { get; set; }
        public ElementReference TableBodyInnerRef { get; set; }

        public double GridColWidthSum { get; set; }

        private readonly IConstantService _constantService;
        private readonly IJsService _jsService;
        private readonly IColumnService _columnService;

        public DotNetObjectReference<ILayoutService> DataGridDotNetRef { get; }

        private readonly Lazy<Task<double>> TableRowHeightLazy;

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

            TableRowHeightLazy = new(() => _jsService.MeasureTableRowHeight(TableTagClass.Value).AsTask());
        }

        private void DataGridInitializerCallback(bool obj)
        {
            Task.Run(async () => await HeightWidthChanged(height: Height.Value, width: Width.Value));
        }

        public bool FitColumnsToTableWidth { get; set; } = false;


        public event Func<DataGridStyleInfo, Task>? DataGridInnerCssStyleChanged = null;

        public double FilterRowHeight { get; private set; }

        public Task<double> TableRowHeight()
        {
            return TableRowHeightLazy.Value;
        }
        public double ActualScrollbarWidth { get; set; }


        public BehaviorSubject<string> OuterStyle { get; } = new BehaviorSubject<string>("");

        public BehaviorSubject<string> FooterStyle { get; } = new BehaviorSubject<string>("");

        public BehaviorSubject<string> Height { get; } = new BehaviorSubject<string>("400px");

        public BehaviorSubject<string> Width { get; } = new BehaviorSubject<string>("");

        public BehaviorSubject<bool> ShowColumnHeader { get; } = new BehaviorSubject<bool>(true);

        public BehaviorSubject<bool> ShowDetailHeader => throw new NotImplementedException();

        public double ScrollbarWidth { get; private set; }

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
            var tBodyWidth = await GridOuterRef.GetElementWidthByRef(await _jsService.JsModule());
            CalculateEmptyColumn(tBodyWidth);
        }

        public async Task InitHeader()
        {
            await GridInnerRef.SynchronizeGridHeaderScroll(await _jsService.JsModule(), _constantService.GridHeaderContainerId);
            if (FilterMode.Value == DataGridFilterMode.FilterRow)
            {
                await GridInnerRef.SynchronizeGridHeaderScroll(await _jsService.JsModule(), _constantService.GridFilterContainerId);
            }

            ScrollbarWidth = await _jsService.MeasureScrollbar();
            FilterRowHeight = await _jsService.MeasureTableFilterHeight(TableTagClass.Value, TheadTagClass.Value, FilterToggleButtonClass.Value);

            GridColWidthSum = 0;
            MinGridWidth.OnNext(0);

            var allColumnsCount = _columnService.AllColumnModels.Count + (ShowDetailRow.Value ? 1 : 0);

            var columnsCountWithAbsoluteWidth = _columnService.AllColumnModels.Count(c => c.WidthInfo.WidthMode == ColumnMetadataWidthInfo.ColumnWidthMode.Absolute) + (ShowDetailRow.Value ? 1 : 0);

            if (columnsCountWithAbsoluteWidth != allColumnsCount)
            {
                FitColumnsToTableWidth = true;
                var widthSumOfAllAbsoluteWidth = _columnService.AllColumnModels
                    .Where(c => c.WidthInfo.WidthMode == ColumnMetadataWidthInfo.ColumnWidthMode.Absolute).Sum(c => c.WidthInfo.WidthValue) +
                    (ShowDetailRow.Value ? _columnService.DetailExpanderColumnModel.WidthInfo.WidthValue : 0d);

                MinGridWidth.OnNext(widthSumOfAllAbsoluteWidth + _columnService.AllColumnModels
                    .Where(c => c.WidthInfo.WidthMode == ColumnMetadataWidthInfo.ColumnWidthMode.Relative).Sum(c => c.WidthInfo.MinWidthValue));
            }
            else
            {
                GridColWidthSum = _columnService.AllColumnModels.Sum(c => c.WidthInfo.WidthValue) +
                    (ShowDetailRow.Value ? _columnService.DetailExpanderColumnModel.WidthInfo.WidthValue : 0d);
            }

            if (FitColumnsToTableWidth)
                await ShowDynamicHeader();
            else
                await ShowFixHeader(0);
        }

        private async Task ShowDynamicHeader()
        {
            try
            {
                double gridInnerWidth = await GridInnerRef.GetElementWidthByRef(await _jsService.JsModule());
                gridInnerWidth = Math.Max(gridInnerWidth, MinGridWidth.Value);

                Dictionary<IColumnModel, TempColumnMetadata> widthList = new Dictionary<IColumnModel, TempColumnMetadata>();

                var absoluteWidthSum = _columnService.AllColumnModels.
                    Where(cmt => cmt.WidthInfo.WidthMode == ColumnMetadataWidthInfo.ColumnWidthMode.Absolute).Sum(cmt => cmt.WidthInfo.WidthValue) +
                    (ShowDetailRow.Value ? _columnService.DetailExpanderColumnModel.WidthInfo.WidthValue : 0d);

                var relativeWidthSum = _columnService.AllColumnModels.Where(cmt => cmt.WidthInfo.WidthMode == ColumnMetadataWidthInfo.ColumnWidthMode.Relative).Sum(cmt => cmt.WidthInfo.WidthValue);

                var restForRelativePercent = (gridInnerWidth - absoluteWidthSum) / gridInnerWidth;

                foreach (var m in _columnService.AllColumnModels.Where(cmt => cmt.WidthInfo.WidthMode == ColumnMetadataWidthInfo.ColumnWidthMode.Relative))
                {
                    double nw = restForRelativePercent * m.WidthInfo.WidthValue / relativeWidthSum;
                    widthList.Add(m, new TempColumnMetadata { VisibleAbsoluteWidthValue = nw * gridInnerWidth, VisiblePercentWidthValue = nw * 100d });
                }

                foreach (var m in _columnService.AllColumnModels.Where(cmt => cmt.WidthInfo.WidthMode == ColumnMetadataWidthInfo.ColumnWidthMode.Absolute))
                {
                    double nw = m.WidthInfo.WidthValue / gridInnerWidth;
                    widthList.Add(m, new TempColumnMetadata { VisibleAbsoluteWidthValue = m.WidthInfo.WidthValue, VisiblePercentWidthValue = nw * 100d });
                }

                foreach (var m in _columnService.AllColumnModels)
                {
                    m.VisibleAbsoluteWidthValue = widthList[m].VisibleAbsoluteWidthValue;
                    m.VisiblePercentWidthValue = widthList[m].VisiblePercentWidthValue;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task ShowFixHeader(int newWidth)
        {
            double gridInnerWidth = 0;

            gridInnerWidth = await GridInnerRef.GetElementWidthByRef(await _jsService.JsModule());

            Dictionary<IColumnModel, TempColumnMetadata> widthList = new Dictionary<IColumnModel, TempColumnMetadata>();

            foreach (var m in _columnService.AllColumnModels.Where(cmt => cmt.WidthInfo.WidthMode == ColumnMetadataWidthInfo.ColumnWidthMode.Absolute))
            {
                double nw = m.WidthInfo.WidthValue / gridInnerWidth;
                widthList.Add(m, new TempColumnMetadata { VisibleAbsoluteWidthValue = m.WidthInfo.WidthValue, VisiblePercentWidthValue = nw * 100d });
            }

            //Werte aus TempObjekt transferieren
            foreach (var m in _columnService.AllColumnModels.Where(widthList.ContainsKey))
            {
                m.VisibleAbsoluteWidthValue = widthList[m].VisibleAbsoluteWidthValue;
                m.VisiblePercentWidthValue = widthList[m].VisiblePercentWidthValue;
            }

            double tBodyWidth = 0;
            if (newWidth == 0)
                tBodyWidth = await GridOuterRef.GetElementWidthByRef(await _jsService.JsModule());
            else
                tBodyWidth = newWidth;

            CalculateEmptyColumn(tBodyWidth);
        }

        private void CalculateEmptyColumn(double tBodyWidth)
        {
            double emptyColWidth = 0;

            if (!FitColumnsToTableWidth)
            {
                emptyColWidth = tBodyWidth - GridColWidthSum - ActualScrollbarWidth - 1;
                emptyColWidth = Math.Max(0, emptyColWidth);
            }

            emptyColWidth = emptyColWidth - (ShowDetailRow.Value ? _columnService.DetailExpanderColumnModel.WidthInfo.WidthValue : 0d);

            _columnService.EmptyColumnModel.VisibleAbsoluteWidthValue = emptyColWidth;

            _columnService.EmptyColumnModel.WidthInfo = new ColumnMetadataWidthInfo { WidthMode = ColumnMetadataWidthInfo.ColumnWidthMode.Absolute, WidthValue = emptyColWidth };
        }

        internal async Task OnDataGridInnerCssStyleChanged()
        {
            if (DataGridInnerCssStyleChanged != null)
            {
                DataGridStyleInfo info = new DataGridStyleInfo
                {
                    CssStyle = OuterStyle.Value,
                    ActualScrollbarWidth = ActualScrollbarWidth
                };
                await DataGridInnerCssStyleChanged.Invoke(info);
            }
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }

        class TempColumnMetadata
        {
            public double VisibleAbsoluteWidthValue { get; set; }
            public double VisiblePercentWidthValue { get; set; }
        }
    }
}
