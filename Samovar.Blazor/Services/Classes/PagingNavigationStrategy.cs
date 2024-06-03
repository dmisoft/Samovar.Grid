using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    internal class PagingNavigationStrategy<T>
        : IPagingNavigationStrategy
    {
        public BehaviorSubject<uint> PageSize { get; } = new BehaviorSubject<uint>(50);

        public BehaviorSubject<uint> PagerSize { get; } = new BehaviorSubject<uint>(10);

        public BehaviorSubject<uint> TotalPageCount { get; set; } = new BehaviorSubject<uint>(0);

        public BehaviorSubject<uint> CurrentPage { get; private set; } = new BehaviorSubject<uint>(0);

        public BehaviorSubject<GridPagerInfo> PagerInfo { get; private set; } = new BehaviorSubject<GridPagerInfo>(GridPagerInfo.Empty);

        public required BehaviorSubject<NavigationStrategyDataLoadingSettings> DataLoadingSettings { get; set; } = new BehaviorSubject<NavigationStrategyDataLoadingSettings>(NavigationStrategyDataLoadingSettings.Empty);

        private readonly IDataSourceService<T> _dataSourceService;

        public PagingNavigationStrategy(
              IInitService initService
            , IDataSourceService<T> dataSourceService)
        {
            _dataSourceService = dataSourceService;
            initService.IsInitialized.Subscribe(DataGridInitializerCallback);
        }

        private void DataGridInitializerCallback(bool obj)
        {
            PageSize.Subscribe(x => {
				uint totalPageCount = (uint)Math.Ceiling(actualItemsCount / (decimal)x);
                if(CurrentPage.Value > totalPageCount)
					CurrentPage.OnNext(totalPageCount);
				TotalPageCount.OnNext(totalPageCount);
			});

			Observable.CombineLatest(
				PagerSize,
                TotalPageCount,
                CurrentPage,
                PagerInfoChanged).Subscribe(PagerInfoSubscriber);

            Observable.CombineLatest(
                PageSize,
                CurrentPage,
                CalculateDataLoadingSetting).Subscribe(DataLoadingSettingSubscriber);

            _dataSourceService.DataQuery.Where(x => x != null).Subscribe(ProcessDataPrequery);
        }

        private void DataLoadingSettingSubscriber(NavigationStrategyDataLoadingSettings loadingSettings)
        {
            DataLoadingSettings.OnNext(loadingSettings);
        }

        private void PagerInfoSubscriber(GridPagerInfo info)
        {
            PagerInfo.OnNext(info);
        }

        private NavigationStrategyDataLoadingSettings CalculateDataLoadingSetting(uint pageSize, uint currentPage)
        {
            if (currentPage == 0)
                return NavigationStrategyDataLoadingSettings.Empty;

            var skipPages = currentPage - 1;
            return new NavigationStrategyDataLoadingSettings(skip: skipPages * pageSize, take: pageSize);
        }

        private int actualItemsCount = 0;
        private GridPagerInfo PagerInfoChanged(uint pagerSize, uint pageCount, uint currentPage)
        {
			if (currentPage == 0)
                return GridPagerInfo.Empty;
            uint startPage = (uint)Math.Ceiling((decimal)currentPage / (decimal)pagerSize) * pagerSize - pagerSize + 1;
            uint endPage = Math.Min(startPage + pagerSize - 1, pageCount);
            return new GridPagerInfo(startPage: startPage, endPage: endPage, currentPage: currentPage, totalPages: pageCount);
        }

        public Task NavigateToNextPage()
        {
            uint newPage = CurrentPage.Value < TotalPageCount.Value ? CurrentPage.Value + 1 : CurrentPage.Value;
            CurrentPage.OnNext(newPage);
            return Task.CompletedTask;
        }

        public Task NavigateToPage(uint pageNumber)
        {
            CurrentPage.OnNext(pageNumber);
            return Task.CompletedTask;
        }

        public Task NavigateToPreviousPage()
        {
            uint newPage = PagerInfo.Value.CurrentPage > 1 ? PagerInfo.Value.CurrentPage - 1 : 1;
            CurrentPage.OnNext(newPage);
            return Task.CompletedTask;
        }

        public Task NavigateToNextPager()
        {
            throw new NotImplementedException();
        }

        public Task NavigateToPreviousPager()
        {
            throw new NotImplementedException();
        }

        public Task NavigateToFirstPage()
        {
            throw new NotImplementedException();
        }

        public Task NavigateToLastPage()
        {
            throw new NotImplementedException();
        }

        public void ProcessDataPrequery<T1>(IQueryable<T1>? data)
        {
            if (data is null)
                return;

            actualItemsCount = data.Count();

            uint newTotalPageCount = (uint)Math.Ceiling(actualItemsCount / (decimal)PageSize.Value);
            TotalPageCount.OnNext(newTotalPageCount);

            uint newCurrentPage = CurrentPage.Value == 0 && newTotalPageCount > 0 ? 1 : Math.Min(newTotalPageCount, CurrentPage.Value);
            CurrentPage.OnNext(newCurrentPage);
        }

    }
}
