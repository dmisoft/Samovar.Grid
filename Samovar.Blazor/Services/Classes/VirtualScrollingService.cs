using Microsoft.JSInterop;
using System;
using System.Globalization;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public class VirtualScrollingService<T>
        : IVirtualScrollingService, IAsyncDisposable
    {
        //public ISubject<NavigationStrategyDataLoadingSettings> DataLoadingSettings { get; set; } = new ParameterSubject<NavigationStrategyDataLoadingSettings>(NavigationStrategyDataLoadingSettings.Empty);

        public DotNetObjectReference<IVirtualScrollingService> DotNetRef { get; }

        public int VisibleItems { get; set; }
        public int ItemsToShow { get; set; } = 10;
        public double DummyRowHeight { get; set; }
        public int DummyItemsCount { get; set; }
        public int TopVisibleDataItemPosition { get; set; }
        public int StartGridItemPosition { get; set; }
        public int EndGridItemPosition { get; set; }

        public double TranslateYOffset { get; private set; } = 0d;
        
        protected double ActualTopOffset = 0;

        private readonly ILayoutService _layoutService;
        private readonly IJsService _jsService;
        private readonly IInitService _initService;
        private readonly IDataSourceService<T> _dataSourceService;

        //private readonly IRepositoryService<T> _repositoryService;
        private readonly IConstantService _constantService;

        public async Task<double> TranslatableDivHeight(int itemCount)
        {
            var translatableDivHeight = await _layoutService.TableRowHeight() * itemCount;
            return translatableDivHeight;
        }

        public BehaviorSubject<string> TranslatableDivHeightValue { get; } = new BehaviorSubject<string>("");

        public BehaviorSubject<DataGridVirtualScrollingInfo> VirtualScrollingInfo { get; } = new BehaviorSubject<DataGridVirtualScrollingInfo>(DataGridVirtualScrollingInfo.Empty);

        public ISubject<IQueryable> Query { get; }

        public VirtualScrollingService(
              ILayoutService layoutService
            , IJsService jsService
            , IInitService initService
            , IDataSourceService<T> dataSourceService
            , IConstantService constantService
            )
        {
            DotNetRef = DotNetObjectReference.Create(this as IVirtualScrollingService);

            _layoutService = layoutService;
            _jsService = jsService;
            _initService = initService;
            _dataSourceService = dataSourceService;
            _constantService = constantService;

            //TODO refactoring 10/2023
            //var sub1 = new Subscription1TaskVoid<bool>(_initService.IsInitialized, myfunc1);
            //sub1.CreateMap();
        }

        private async Task myfunc1(bool arg)
        {
            //int cnt =  _repositoryService.TotalItemsCount.SubjectValue == null ? 0 : _repositoryService.TotalItemsCount.SubjectValue.Count();

            //double divHeightValue = await TranslatableDivHeight(cnt);

            //double innerGridHeight = await _jsService.GetInnerGridHeight();

            //string divHeight = $"{Math.Max(divHeightValue, innerGridHeight).ToString(CultureInfo.InvariantCulture)}px";

            //TranslatableDivHeightValue.OnNextParameterValue(divHeight);

            //VirtualScrollingInfo.OnNextParameterValue(new DataGridVirtualScrollingInfo(0d, 0d, divHeight));

            //var sub2 = new Subscription1TaskVoid<IQueryable<T>>(_repositoryService.TotalItemsCount, myfunc2);
            //sub2.CreateMap();
        }

        //private async Task myfunc2(IQueryable<T> query)
        //{
        //    await _jsService.ScrollInnerGridToTop();

        //    double divHeightValue = await TranslatableDivHeight(query.Count());

        //    double innerGridHeight = await _jsService.GetInnerGridHeight();

        //    string divHeight = $"{Math.Max(divHeightValue, innerGridHeight).ToString(CultureInfo.InvariantCulture)}px";

        //    TranslatableDivHeightValue.OnNextParameterValue(divHeight);
            
        //    await ProcessVirtualScrolling(0);
        //}


        [JSInvokable]
        public async Task Js_InnerGrid_AfterScroll(double scrollTop)
        {
            await ProcessVirtualScrolling(scrollTop);
        }

        protected async Task ProcessVirtualScrolling(double scrollTop)
        {
            double rowHeight = await _layoutService.TableRowHeight();
            double innerGridHeight = await _jsService.GetInnerGridHeight();

            int visibleItems = (int)Math.Round(innerGridHeight / rowHeight, 2, MidpointRounding.AwayFromZero) + 1;
            int skip = (int)(scrollTop / rowHeight);

            _dataSourceService.DataLoadingSettings.OnNext(new NavigationStrategyDataLoadingSettings(skip: skip, take: visibleItems));

            VirtualScrollingInfo.OnNext(new DataGridVirtualScrollingInfo(0d, skip * rowHeight, TranslatableDivHeightValue.Value));
        }

        public async Task Activate()
        {
            await _jsService.AttachOnScrollollingEvent(_constantService.InnerGridId, DotNetRef);
        }

        public async Task Deactivate()
        {
            await _jsService.DetachOnScrollollingEvent (_constantService.InnerGridId, DotNetRef);
        }

        public async ValueTask DisposeAsync()
        {
            await Deactivate();
        }

        public async Task ProcessDataPrequery<T1>(IQueryable<T1> data)
        {
            await _jsService.ScrollInnerGridToTop();

            double divHeightValue = await TranslatableDivHeight(data.Count());

            double innerGridHeight = await _jsService.GetInnerGridHeight();

            string divHeight = $"{Math.Max(divHeightValue, innerGridHeight).ToString(CultureInfo.InvariantCulture)}px";

            TranslatableDivHeightValue.OnNext(divHeight);

            await ProcessVirtualScrolling(0);
        }
    }
}
