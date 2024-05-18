using Microsoft.JSInterop;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public class VirtualScrollingNavigationStrategy<T>
        : IVirtualScrollingNavigationStrategy, IAsyncDisposable
    {
        public required BehaviorSubject<NavigationStrategyDataLoadingSettings> DataLoadingSettings { get; set; } = new BehaviorSubject<NavigationStrategyDataLoadingSettings>(NavigationStrategyDataLoadingSettings.Empty);
        public BehaviorSubject<double> ScrollTop { get; private set; } = new BehaviorSubject<double>(0);
        public DotNetObjectReference<IVirtualScrollingNavigationStrategy> DotNetRef { get; }

        public int VisibleItems { get; set; }
        public int ItemsToShow { get; set; } = 10;

        public double TopPlaceholderRowHeight { get; set; } = 0;
        public double BottomPlaceholderRowHeight { get; set; } = 0;

        public int TopVisibleDataItemPosition { get; set; }
        public int StartGridItemPosition { get; set; }
        public int EndGridItemPosition { get; set; }

        public double TranslateYOffset { get; private set; } = 0d;

        protected double ActualTopOffset = 0;
        private double _translatableDivHeight;
        private readonly IJsService _jsService;

        private readonly IConstantService _constantService;

        public Task<double> GetTranslatableDivHeight(int itemCount)
        {
            var translatableDivHeight = 34.3 * itemCount;
            return Task.FromResult(translatableDivHeight);
        }

        public BehaviorSubject<DataGridVirtualScrollingInfo> VirtualScrollingInfo { get; } = new BehaviorSubject<DataGridVirtualScrollingInfo>(DataGridVirtualScrollingInfo.Empty);

        public VirtualScrollingNavigationStrategy(
              ILayoutService layoutService
            , IJsService jsService
            , IInitService initService
            , IDataSourceService<T> dataSourceService
            , IConstantService constantService
            )
        {
            DotNetRef = DotNetObjectReference.Create(this as IVirtualScrollingNavigationStrategy);

            _jsService = jsService;
            _constantService = constantService;

            initService.IsInitialized.Subscribe(async (val) => await DataGridInitializerCallback());
        }

        private async Task DataGridInitializerCallback()
        {
            await Activate();
        }


        [JSInvokable]
        public Task Js_InnerGrid_AfterScroll(double scrollTop)
        {
            ScrollTop.OnNext(scrollTop);
            return Task.CompletedTask;
        }

        protected async Task<NavigationStrategyDataLoadingSettings> GetDataLoadingSettings(double scrollTop)
        {
            double rowHeight = 34.3;
            double scrollContainerHeight = await _jsService.GetInnerGridHeight();

            int visibleItems = (int)Math.Round(scrollContainerHeight / rowHeight, 2, MidpointRounding.AwayFromZero) + 1;
            int skip = (int)(scrollTop / rowHeight);

            var topPlaceholderHeight = skip * rowHeight;
            var bottomPlaceholderHeight = _translatableDivHeight - visibleItems * rowHeight - topPlaceholderHeight;


            VirtualScrollingInfo.OnNext(
                new DataGridVirtualScrollingInfo(
                    offsetX: 0d,
                    offsetY: scrollTop,// skip * rowHeight,
                    contentContainerHeight: _translatableDivHeight,
                    topPlaceholderHeight: topPlaceholderHeight,
                    bottomPlaceholderHeight: bottomPlaceholderHeight,
                    scrollContainerHeight: scrollContainerHeight));

            return new NavigationStrategyDataLoadingSettings(skip: skip, take: visibleItems);
        }

        public async Task Activate()
        {
            await _jsService.AttachOnScrollollingEvent(_constantService.InnerGridId, DotNetRef);
        }

        public async Task Deactivate()
        {
            await _jsService.DetachOnScrollollingEvent(_constantService.InnerGridId, DotNetRef);
        }

        public async ValueTask DisposeAsync()
        {
            await Deactivate();
        }

        public async Task ProcessDataPrequery<T1>(IQueryable<T1>? data)
        {
            if (data is null)
                return;

            await _jsService.ScrollInnerGridToTop();

            double divHeightValue = await GetTranslatableDivHeight(data.Count());

            double innerGridHeight = await _jsService.GetInnerGridHeight();

            _translatableDivHeight = Math.Max(divHeightValue, innerGridHeight);

            ScrollTop.OnNext(0);
        }
    }
}
