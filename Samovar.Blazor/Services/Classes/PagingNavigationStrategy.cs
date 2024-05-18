using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    internal class PagingNavigationStrategy<T>
        : IPagingNavigationStrategy
    {
        public BehaviorSubject<int> PageSize { get; } = new BehaviorSubject<int>(50);

        public BehaviorSubject<int> PagerSize { get; } = new BehaviorSubject<int>(10);

        public BehaviorSubject<int> TotalPageCount { get; set; } = new BehaviorSubject<int>(0);

        public BehaviorSubject<int> CurrentPage { get; private set; } = new BehaviorSubject<int>(0);

        public BehaviorSubject<DataGridPagerInfo> PagerInfo { get; private set; } = new BehaviorSubject<DataGridPagerInfo>(DataGridPagerInfo.Empty);

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

        private void PagerInfoSubscriber(DataGridPagerInfo info)
        {
            PagerInfo.OnNext(info);
        }

        private NavigationStrategyDataLoadingSettings CalculateDataLoadingSetting(int pageSize, int currentPage)
        {
            if (currentPage == 0)
                return NavigationStrategyDataLoadingSettings.Empty;

            var skipPages = currentPage - 1;
            return new NavigationStrategyDataLoadingSettings(skip: skipPages * pageSize, take: pageSize);
        }

        private DataGridPagerInfo PagerInfoChanged(int pagerSize, int pageCount, int currentPage)
        {
            if (currentPage == 0)
                return DataGridPagerInfo.Empty;
            int startPage = (int)Math.Ceiling((decimal)currentPage / (decimal)pagerSize) * pagerSize - pagerSize + 1;
            int endPage = Math.Min(startPage + pagerSize - 1, pageCount);
            return new DataGridPagerInfo(startPage: startPage, endPage: endPage, currentPage: currentPage, totalPages: pageCount);
        }

        public Task NavigateToNextPage()
        {
            int newPage = CurrentPage.Value < TotalPageCount.Value ? CurrentPage.Value + 1 : CurrentPage.Value;
            CurrentPage.OnNext(newPage);
            return Task.CompletedTask;
        }

        public Task NavigateToPage(int pageNumber)
        {
            CurrentPage.OnNext(pageNumber);
            return Task.CompletedTask;
        }

        public Task NavigateToPreviousPage()
        {
            int newPage = PagerInfo.Value.CurrentPage > 1 ? PagerInfo.Value.CurrentPage - 1 : 1;
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

            int items = data.Count();

            int newTotalPageCount = (int)Math.Ceiling(items / (decimal)PageSize.Value);
            TotalPageCount.OnNext(newTotalPageCount);

            int newCurrentPage = CurrentPage.Value == 0 && newTotalPageCount > 0 ? 1 : Math.Min(newTotalPageCount, CurrentPage.Value);
            CurrentPage.OnNext(newCurrentPage);
        }

    }
}
