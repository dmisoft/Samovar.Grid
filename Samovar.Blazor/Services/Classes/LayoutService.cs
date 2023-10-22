using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public class LayoutService
        : ILayoutService, IDisposable
    {
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

        private IConstantService _constantService;
        private IJsService _jsService;
        private IInitService _initService;
        private IColumnService _columnService;

        public DotNetObjectReference<ILayoutService> DataGridDotNetRef { get; }

        private readonly Lazy<Task<double>> ScrollbarWidthLazy = null;
        private readonly Lazy<Task<double>> TableRowHeightLazy = null;

        public LayoutService(
              IConstantService constantService
            , IJsService jsService
            , IInitService initService
            , IColumnService columnService)
        {
            _constantService = constantService;
            _jsService = jsService;
            _initService = initService;
            _columnService = columnService;

            //_initService.IsInitialized.Subscribe(new SmObserver<bool>(new SmObserverDispatcher<bool>(DataGridInitializerCallback)));
            _initService.IsInitialized.Subscribe(DataGridInitializerCallback);

            DataGridDotNetRef = DotNetObjectReference.Create(this as ILayoutService);

            ScrollbarWidthLazy = new(() => _jsService.MeasureScrollbar().AsTask());

            TableRowHeightLazy = new(() => _jsService.MeasureTableRowHeight(TableTagClass.Value).AsTask());
        }

        private void DataGridInitializerCallback(bool obj)
        {
            //TODO refactoring 10/2023
            //var sub1 = new Subscription2TaskVoid<string, string>(Height, Width, HeightWidthChanged);
            //sub1.CreateMap();

            //Standard values
            Task.Run(async () => await HeightWidthChanged(height: Height.Value, width: Width.Value));
        }

        public bool FitColumnsToTableWidth { get; set; } = false;


        public event Func<DataGridStyleInfo, Task> DataGridInnerCssStyleChanged;

        //internal double MinGridWidth { get; set; }


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

            await CheckScrollBarWidth();
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

            var allCols = _columnService.AllColumnModels.Count() + (ShowDetailRow.Value ? 1 : 0);
            
            var absCols = _columnService.AllColumnModels.Where(c => c.WidthInfo.WidthMode == ColumnMetadataWidthInfo.ColumnWidthMode.Absolute).Count() + (ShowDetailRow.Value ? 1 : 0); ;
            
            var relCols = _columnService.AllColumnModels.Where(c => c.WidthInfo.WidthMode == ColumnMetadataWidthInfo.ColumnWidthMode.Relative).Count();

            if (absCols != allCols)
            {
                FitColumnsToTableWidth = true;
                var absColsWidthSum = _columnService.AllColumnModels
                    .Where(c => c.WidthInfo.WidthMode == ColumnMetadataWidthInfo.ColumnWidthMode.Absolute).Sum(c => c.WidthInfo.WidthValue) +
                    (ShowDetailRow.Value ? _columnService.DetailExpanderColumnModel.WidthInfo.WidthValue : 0d);

                MinGridWidth.OnNext(absColsWidthSum + _columnService.AllColumnModels
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
                CultureInfo ci = CultureInfo.InvariantCulture;

                double gridInnerWidth = await GridInnerRef.GetElementWidthByRef(await _jsService.JsModule());
                gridInnerWidth = Math.Max(gridInnerWidth, MinGridWidth.Value);

                Dictionary<IColumnModel, TempColumnMetadata> widthList = new Dictionary<IColumnModel, TempColumnMetadata>();

                var absoluteWidthSum = _columnService.AllColumnModels.
                    Where(cmt => cmt.WidthInfo.WidthMode == ColumnMetadataWidthInfo.ColumnWidthMode.Absolute).Sum(cmt => cmt.WidthInfo.WidthValue) +
                    (ShowDetailRow.Value ? _columnService.DetailExpanderColumnModel.WidthInfo.WidthValue : 0d);

                var relativeWidthSum = _columnService.AllColumnModels.Where(cmt => cmt.WidthInfo.WidthMode == ColumnMetadataWidthInfo.ColumnWidthMode.Relative).Sum(cmt => cmt.WidthInfo.WidthValue);

                var restForRelative = gridInnerWidth - absoluteWidthSum;
                
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
                //Test
                var abs = widthList.Sum(cmt => cmt.Value.VisibleAbsoluteWidthValue);
                var rel = widthList.Sum(cmt => cmt.Value.VisiblePercentWidthValue);

                //Werte aus TempObjekt transferieren
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
            try
            {
                CultureInfo ci = CultureInfo.InvariantCulture;

                double gridInnerWidth = await GridInnerRef.GetElementWidthByRef(await _jsService.JsModule());
                Dictionary<IColumnModel, TempColumnMetadata> widthList = new Dictionary<IColumnModel, TempColumnMetadata>();

                foreach (var m in _columnService.AllColumnModels.Where(cmt => cmt.WidthInfo.WidthMode == ColumnMetadataWidthInfo.ColumnWidthMode.Absolute))
                {
                    double nw = m.WidthInfo.WidthValue / gridInnerWidth;
                    widthList.Add(m, new TempColumnMetadata { VisibleAbsoluteWidthValue = m.WidthInfo.WidthValue, VisiblePercentWidthValue = nw * 100d });
                }

                //Werte aus TempObjekt transferieren
                foreach (var m in _columnService.AllColumnModels)
                {
                    if (widthList.ContainsKey(m))
                    {
                        m.VisibleAbsoluteWidthValue = widthList[m].VisibleAbsoluteWidthValue;
                        m.VisiblePercentWidthValue = widthList[m].VisiblePercentWidthValue;
                    }
                }

                double tBodyWidth = 0;
                if (newWidth == 0)
                    tBodyWidth = await GridOuterRef.GetElementWidthByRef(await _jsService.JsModule());
                else
                    tBodyWidth = newWidth;

                CalculateEmptyColumn(tBodyWidth);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
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

        //TODO Trigger: on window size changed
        public async Task<bool> CheckScrollBarWidth()
        {
            try
            {
                double tempActualScrollbarWidth = 0d;

                if (await (await _jsService.JsModule()).InvokeAsync<bool>("isScrollbarVisible", new object[] { _constantService.InnerGridId }))
                {
                    tempActualScrollbarWidth = ScrollbarWidth;
                }

                var retVal = tempActualScrollbarWidth != ActualScrollbarWidth ? true : false;
                
                ActualScrollbarWidth = tempActualScrollbarWidth;
                
                await OnDataGridInnerCssStyleChanged();

                return retVal;
            }
            catch
            {
            }

            return false;
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



        public void Dispose()
        {

        }

        public Task<double> ScrollbarWidth2()
        {
            return ScrollbarWidthLazy.Value;
        }

        class TempColumnMetadata
        {
            public double VisibleAbsoluteWidthValue { get; set; }
            public double VisiblePercentWidthValue { get; set; }
        }
    }
}
