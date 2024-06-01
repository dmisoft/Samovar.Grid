using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Samovar.Blazor.Columns;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public class LayoutService
        : ILayoutService, IAsyncDisposable
    {
        public BehaviorSubject<ColumnResizeMode> ColumnResizeMode { get; } = new BehaviorSubject<ColumnResizeMode>(Columns.ColumnResizeMode.None);

        public BehaviorSubject<string> SelectedRowClass { get; } = new BehaviorSubject<string>("bg-warning");

        public BehaviorSubject<string> TableTagClass { get; } = new BehaviorSubject<string>("table table-bordered");

        public BehaviorSubject<string> TheadTagClass { get; } = new BehaviorSubject<string>("table-light");

        public BehaviorSubject<double> MinGridWidth { get; } = new BehaviorSubject<double>(0d);

        public BehaviorSubject<bool> ShowDetailRow { get; } = new BehaviorSubject<bool>(false);

        public BehaviorSubject<string> PaginationClass { get; } = new BehaviorSubject<string>("pagination");
        public BehaviorSubject<string> FilterToggleButtonClass { get; } = new BehaviorSubject<string>("btn btn-secondary");

        public BehaviorSubject<bool> ShowFilterRow { get; } = new BehaviorSubject<bool>(false);

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
                        (ShowDetailRow.Value ? _columnService.DetailExpanderColumnModel.Width.Value : 0d);
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
                    string footerStyle = "";
                    if (!string.IsNullOrEmpty(_[0]))
                    {
                        outerStyle += $"width:{_[0]};";
                        footerStyle = $"width:{_[0]};";
                    }

                    return Task.FromResult(new GridStyleInfo { CssStyle = outerStyle, FooterStyle = footerStyle });
                });
        }

        private void DataGridInitializerCallback(bool obj)
        {
            Task.Run(async () => await HeightWidthChanged(height: Height.Value, width: Width.Value));
        }

        public event Func<GridStyleInfo, Task>? DataGridInnerCssStyleChanged = null;

        public double FilterRowHeight { get; private set; }

        public BehaviorSubject<string> OuterStyle { get; } = new BehaviorSubject<string>("");

        public BehaviorSubject<string> FooterStyle { get; } = new BehaviorSubject<string>("");

        public BehaviorSubject<string> Height { get; } = new BehaviorSubject<string>("400px");

        public BehaviorSubject<string> Width { get; } = new BehaviorSubject<string>("");

        public BehaviorSubject<bool> ShowColumnHeader { get; } = new BehaviorSubject<bool>(true);

        public BehaviorSubject<bool> ShowDetailHeader => throw new NotImplementedException();

        public IObservable<Task<GridStyleInfo>> DataGridInnerStyle { get; }
        public bool OriginalColumnsWidthChanged { get; set; }

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
            await InitHeader();
        }

        public async Task InitHeader()
        {
            if (OriginalColumnsWidthChanged)
                return;

            await GridInnerRef.SynchronizeGridHeaderScroll(await _jsService.JsModule(), _constantService.GridHeaderContainerId);
            if (FilterMode.Value == GridFilterMode.FilterRow)
            {
                await GridInnerRef.SynchronizeGridHeaderScroll(await _jsService.JsModule(), _constantService.GridFilterContainerId);
            }

            FilterRowHeight = await _jsService.MeasureTableFilterHeight(TableTagClass.Value, TheadTagClass.Value, FilterToggleButtonClass.Value);

            await CaculateHeader();
        }

        private async Task CaculateHeader()
        {
            double gridInnerWidth = await GridInnerRef.GetElementWidthByRef(await _jsService.JsModule())-1;
            var tBodyWidth = await GridOuterRef.GetElementWidthByRef(await _jsService.JsModule());

            var declaratedAbsoluteColumnsWidthSum = _columnService.DeclarativeColumnModels.
                Where(cmt => cmt.DeclaratedWidthMode == DeclarativeColumnWidthMode.Absolute)
                .Sum(cmt => cmt.DeclaratedWidth) + (ShowDetailRow.Value ? _columnService.DetailExpanderColumnModel.DeclaratedWidth : 0d);

            var relativePortionSum = _columnService.DeclarativeColumnModels
                .Where(cmt => cmt.DeclaratedWidthMode == DeclarativeColumnWidthMode.Relative)
                .Sum(cmt => cmt.DeclaratedWidth);

            var absoluteColumnsWidthSumForRelative = gridInnerWidth - declaratedAbsoluteColumnsWidthSum;

            var emptyColWidth = Math.Max(tBodyWidth - declaratedAbsoluteColumnsWidthSum - absoluteColumnsWidthSumForRelative, 0);
            var portionValue = (gridInnerWidth - declaratedAbsoluteColumnsWidthSum) / relativePortionSum;

            _columnService.EmptyColumnModel.Width.OnNext(emptyColWidth);

            Dictionary<IColumnModel, double> widthList = new Dictionary<IColumnModel, double>();

            foreach (var m in _columnService.DeclarativeColumnModels.Where(cmt => cmt.DeclaratedWidthMode == DeclarativeColumnWidthMode.Relative))
            {
                double nw = portionValue * m.DeclaratedWidth;
                widthList.Add(m, nw);
            }

            foreach (var m in _columnService.DeclarativeColumnModels.Where(cmt => cmt.DeclaratedWidthMode == DeclarativeColumnWidthMode.Absolute))
            {
                widthList.Add(m, m.DeclaratedWidth);
            }

            foreach (var m in _columnService.AllColumnModels)
            {
                m.Width.OnNext(widthList[m]);
            }

            if(ShowDetailRow.Value)
            {
                _columnService.DetailExpanderColumnModel.Width.OnNext(_columnService.DetailExpanderColumnModel.DeclaratedWidth);
            }
        }

        internal async Task OnDataGridInnerCssStyleChanged()
        {
            if (DataGridInnerCssStyleChanged != null)
            {
                GridStyleInfo info = new GridStyleInfo
                {
                    CssStyle = OuterStyle.Value,
                    ActualScrollbarWidth = 0
                };
                await DataGridInnerCssStyleChanged.Invoke(info);
            }
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
