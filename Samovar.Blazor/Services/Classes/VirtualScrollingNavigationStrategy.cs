using Microsoft.JSInterop;
using System;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public class VirtualScrollingNavigationStrategy<T>
        : IVirtualScrollingNavigationStrategy, IAsyncDisposable
    {
        public IObservable<NavigationStrategyDataLoadingSettings> DataLoadingSettings { get; private set; }// = new BehaviorSubject<NavigationStrategyDataLoadingSettings>(NavigationStrategyDataLoadingSettings.Empty);
        private AsyncSubject<double> ScrollTop { get; set; } //= new AsyncSubject <double>(0);

        public DotNetObjectReference<IVirtualScrollingNavigationStrategy> DotNetRef { get; }

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

        public VirtualScrollingNavigationStrategy(
              ILayoutService layoutService
            , IJsService jsService
            , IInitService initService
            , IDataSourceService<T> dataSourceService
            , IConstantService constantService
            )
        {
            DotNetRef = DotNetObjectReference.Create(this as IVirtualScrollingNavigationStrategy);

            _layoutService = layoutService;
            _jsService = jsService;
            _initService = initService;
            _dataSourceService = dataSourceService;
            _constantService = constantService;

            _initService.IsInitialized.Subscribe(async (val) => await DataGridInitializerCallback(val));

            //TODO refactoring 10/2023
            //var sub1 = new Subscription1TaskVoid<bool>(_initService.IsInitialized, myfunc1);
            //sub1.CreateMap();
        }

        private Task DataGridInitializerCallback(bool val)
        {
            //DataLoadingSettings = Observable.FromAsyncPattern<NavigationStrategyDataLoadingSettings>(async (p)=> await Js_InnerGrid_AfterScroll11(0));
            return Task.CompletedTask;
        }

        //private async Task DataGridInitializerCallback(bool obj)
        //{
        //    int cnt =  _repositoryService.TotalItemsCount.SubjectValue == null ? 0 : _repositoryService.TotalItemsCount.SubjectValue.Count();

        //    double divHeightValue = await TranslatableDivHeight(cnt);

        //    double innerGridHeight = await _jsService.GetInnerGridHeight();

        //    string divHeight = $"{Math.Max(divHeightValue, innerGridHeight).ToString(CultureInfo.InvariantCulture)}px";

        //    TranslatableDivHeightValue.OnNext(divHeight);

        //    VirtualScrollingInfo.OnNext(new DataGridVirtualScrollingInfo(0d, 0d, divHeight));

        //    _repositoryService.TotalItemsCount.
        //    //var sub2 = new Subscription1TaskVoid<IQueryable<T>>(_repositoryService.TotalItemsCount, myfunc2);
        //    //sub2.CreateMap();
        //}
        
        

        [JSInvokable]
        public async Task<NavigationStrategyDataLoadingSettings> Js_InnerGrid_AfterScroll(double scrollTop)
        {
            return await ProcessVirtualScrolling(scrollTop);
        }
        protected async Task<NavigationStrategyDataLoadingSettings> GetDataLoadingSettings(double obj)
        {
            double scrollTop = (double)obj;
            double rowHeight = await _layoutService.TableRowHeight();
            double innerGridHeight = await _jsService.GetInnerGridHeight();

            int visibleItems = (int)Math.Round(innerGridHeight / rowHeight, 2, MidpointRounding.AwayFromZero) + 1;
            int skip = (int)(scrollTop / rowHeight);

            return  new NavigationStrategyDataLoadingSettings(skip: skip, take: visibleItems);

            //VirtualScrollingInfo.OnNext(new DataGridVirtualScrollingInfo(0d, skip * rowHeight, TranslatableDivHeightValue.Value));
        }

        protected async Task<NavigationStrategyDataLoadingSettings> ProcessVirtualScrolling(double scrollTop)
        {
            double rowHeight = await _layoutService.TableRowHeight();
            double innerGridHeight = await _jsService.GetInnerGridHeight();

            int visibleItems = (int)Math.Round(innerGridHeight / rowHeight, 2, MidpointRounding.AwayFromZero) + 1;
            int skip = (int)(scrollTop / rowHeight);

            return new NavigationStrategyDataLoadingSettings(skip: skip, take: visibleItems);
            //TODO refactoring
            //DataLoadingSettings.OnNext(new NavigationStrategyDataLoadingSettings(skip: skip, take: visibleItems));

            //VirtualScrollingInfo.OnNext(new DataGridVirtualScrollingInfo(0d, skip * rowHeight, TranslatableDivHeightValue.Value));
        }

        private Task<object> test(object val)
        {
            throw new NotImplementedException();
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
