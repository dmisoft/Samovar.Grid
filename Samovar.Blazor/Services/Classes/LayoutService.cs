using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public class LayoutService
        : ILayoutService, IDisposable
    {
        public ISubject<string> SelectedRowClass { get; } = new ParameterSubject<string>("bg-warning");

        public ISubject<string> TableTagClass { get; } = new ParameterSubject<string>("table table-bordered");

        public ISubject<string> TheadTagClass { get; } = new ParameterSubject<string>("table-light");

        public ISubject<double> MinGridWidth { get; } = new ParameterSubject<double>(0d);

        public ISubject<bool> ShowDetailRow { get; } = new ParameterSubject<bool>(false);

        public ISubject<string> PaginationClass { get; } = new ParameterSubject<string>("pagination");
        public ISubject<string> FilterToggleButtonClass { get; } = new ParameterSubject<string>("btn btn-secondary");

        public IObservable<bool> ShowFilterRow { get; } = new ParameterSubject<bool>(false);

        public ISubject<DataGridFilterMode> FilterMode { get; } = new ParameterSubject<DataGridFilterMode>(DataGridFilterMode.None);

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

            _initService.IsInitialized.Subscribe(new SmObserver<bool>(new SmObserverDispatcher<bool>(DataGridInitializerCallback)));

            DataGridDotNetRef = DotNetObjectReference.Create(this as ILayoutService);

            ScrollbarWidthLazy = new(() => _jsService.MeasureScrollbar().AsTask());

            TableRowHeightLazy = new(() => _jsService.MeasureTableRowHeight(TableTagClass.SubjectValue).AsTask());
        }

        private void DataGridInitializerCallback(bool obj)
        {
            var sub1 = new Subscription2TaskVoid<string, string>(Height, Width, HeightWidthChanged);
            sub1.CreateMap();

            //Standard values
            Task.Run(async () => await HeightWidthChanged(height: Height.SubjectValue, width: Width.SubjectValue));
            //Height.OnNextParameterValue("400px");
            //Width.OnNextParameterValue("");
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


        public ISubject<string> OuterStyle { get; } = new ParameterSubject<string>("");

        public ISubject<string> FooterStyle { get; } = new ParameterSubject<string>("");

        public ISubject<string> Height { get; } = new ParameterSubject<string>("400px");

        public ISubject<string> Width { get; } = new ParameterSubject<string>("");

        public ISubject<bool> ShowColumnHeader { get; } = new ParameterSubject<bool>(true);

        public ISubject<bool> ShowDetailHeader => throw new NotImplementedException();

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

            OuterStyle.OnNextParameterValue(outerStyle);
            FooterStyle.OnNextParameterValue(footerStyle);
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
            if (FilterMode.SubjectValue == DataGridFilterMode.FilterRow)
            {
                await GridInnerRef.SynchronizeGridHeaderScroll(await _jsService.JsModule(), _constantService.GridFilterContainerId);
            }

            ScrollbarWidth = await _jsService.MeasureScrollbar();
            FilterRowHeight = await _jsService.MeasureTableFilterHeight(TableTagClass.SubjectValue, TheadTagClass.SubjectValue, FilterToggleButtonClass.SubjectValue);

            GridColWidthSum = 0;
            MinGridWidth.OnNextParameterValue(0);

            var allCols = _columnService.AllColumnModels.Count() + (ShowDetailRow.SubjectValue ? 1 : 0);
            
            var absCols = _columnService.AllColumnModels.Where(c => c.WidthInfo.WidthMode == ColumnMetadataWidthInfo.ColumnWidthMode.Absolute).Count() + (ShowDetailRow.SubjectValue ? 1 : 0); ;
            
            var relCols = _columnService.AllColumnModels.Where(c => c.WidthInfo.WidthMode == ColumnMetadataWidthInfo.ColumnWidthMode.Relative).Count();

            if (absCols != allCols)
            {
                FitColumnsToTableWidth = true;
                var absColsWidthSum = _columnService.AllColumnModels
                    .Where(c => c.WidthInfo.WidthMode == ColumnMetadataWidthInfo.ColumnWidthMode.Absolute).Sum(c => c.WidthInfo.WidthValue) +
                    (ShowDetailRow.SubjectValue ? _columnService.DetailExpanderColumnModel.WidthInfo.WidthValue : 0d);

                MinGridWidth.OnNextParameterValue(absColsWidthSum + _columnService.AllColumnModels
                    .Where(c => c.WidthInfo.WidthMode == ColumnMetadataWidthInfo.ColumnWidthMode.Relative).Sum(c => c.WidthInfo.MinWidthValue));
            }
            else
            {
                GridColWidthSum = _columnService.AllColumnModels.Sum(c => c.WidthInfo.WidthValue) +
                    (ShowDetailRow.SubjectValue ? _columnService.DetailExpanderColumnModel.WidthInfo.WidthValue : 0d);
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
                gridInnerWidth = Math.Max(gridInnerWidth, MinGridWidth.SubjectValue);

                Dictionary<IColumnModel, TempColumnMetadata> widthList = new Dictionary<IColumnModel, TempColumnMetadata>();

                var absoluteWidthSum = _columnService.AllColumnModels.
                    Where(cmt => cmt.WidthInfo.WidthMode == ColumnMetadataWidthInfo.ColumnWidthMode.Absolute).Sum(cmt => cmt.WidthInfo.WidthValue) +
                    (ShowDetailRow.SubjectValue ? _columnService.DetailExpanderColumnModel.WidthInfo.WidthValue : 0d);

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

            emptyColWidth = emptyColWidth - (ShowDetailRow.SubjectValue ? _columnService.DetailExpanderColumnModel.WidthInfo.WidthValue : 0d);

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
                    CssStyle = OuterStyle.SubjectValue,
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
