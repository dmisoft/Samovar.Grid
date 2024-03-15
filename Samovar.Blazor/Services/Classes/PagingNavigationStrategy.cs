﻿using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

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

        public IObservable<Task<NavigationStrategyDataLoadingSettings>> DataLoadingSettings { get; private set; } //= new BehaviorSubject<NavigationStrategyDataLoadingSettings>(NavigationStrategyDataLoadingSettings.Empty);

        //TODO Refactoring 10/2023
        //Subscription2<int, int, NavigationStrategyDataLoadingSettings> pagingSettingsSubscription;
        private readonly IInitService _initService;
        private readonly IDataSourceService<T> _dataSourceService;

        public PagingNavigationStrategy(
              IInitService initService
            , IDataSourceService<T> dataSourceService)
        {
            _initService = initService;
            _dataSourceService = dataSourceService;
            _initService.IsInitialized.Subscribe(DataGridInitializerCallback);
        }

        private void DataGridInitializerCallback(bool obj)
        {
            var pagingInfoObservable = Observable.CombineLatest(
                PagerSize,
                TotalPageCount,
                CurrentPage,
                PagerInfoChanged).Subscribe(PagerInfoSubscriber);

            DataLoadingSettings = Observable.CombineLatest(
                PageSize,
                CurrentPage,
                CalculateDataLoadingSetting);//.Subscribe(LoadingSettingsSubscriber);

            _dataSourceService.DataQuery.Where(x => x != null).Subscribe(ProcessDataPrequery);
        }

        private void PagerInfoSubscriber(DataGridPagerInfo info)
        {
            PagerInfo.OnNext(info);
        }

        //private void LoadingSettingsSubscriber(NavigationStrategyDataLoadingSettings obj)
        //{
        //    DataLoadingSettings.OnNext(obj);
        //}

        private Task<NavigationStrategyDataLoadingSettings> CalculateDataLoadingSetting(int pageSize, int currentPage)
        {
            //ITaskObservable
            if (currentPage == 0)
                return Task.FromResult(NavigationStrategyDataLoadingSettings.Empty);
            
            var skipPages = currentPage - 1;
            return Task.FromResult(new NavigationStrategyDataLoadingSettings(skip: skipPages * pageSize, take: pageSize));
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

        public Task NavigateToPage(int newPage)
        {
            CurrentPage.OnNext(newPage);
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

        public void ProcessDataPrequery<T1>(IQueryable<T1> data)
        {
            int items = data.Count();

            int newTotalPageCount = (int)Math.Ceiling(items / (decimal)PageSize.Value);
            TotalPageCount.OnNext(newTotalPageCount);

            int newCurrentPage = CurrentPage.Value == 0 && newTotalPageCount > 0 ? 1: Math.Min(newTotalPageCount, CurrentPage.Value);
            CurrentPage.OnNext(newCurrentPage);
        }
    }
}
