using System;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    internal class PagingNavigationStrategy<T>
        : IPagingNavigationStrategy
    {
        public BehaviorSubject<int> PageSize { get; } = new BehaviorSubject<int>(50);

        public BehaviorSubject<int> PagerSize { get; } = new BehaviorSubject<int>(10);

        public BehaviorSubject<int> PageCount { get; set; } = new BehaviorSubject<int>(0);

        public BehaviorSubject<int> CurrentPage { get; private set; } = new BehaviorSubject<int>(0);

        public BehaviorSubject<DataGridPagerInfo> PagerInfo { get; private set; } = new BehaviorSubject<DataGridPagerInfo>(DataGridPagerInfo.Empty);

        //TODO Refactoring 10/2023
        //Subscription2<int, int, NavigationStrategyDataLoadingSettings> pagingSettingsSubscription;
        private readonly IInitService _initService;
        private readonly IDataSourceService<T> _dataSourceService;

        public PagingNavigationStrategy(IInitService initService, IDataSourceService<T> dataSourceService)
        {
            _initService = initService;
            _dataSourceService = dataSourceService;
            _initService.IsInitialized.Subscribe(DataGridInitializerCallback);

            //TODO Refactoring 10/2023
            //PagerInfo = new Subscription3<int, int, int, DataGridPagerInfo>(PagerSize, PageCount, CurrentPage, PagerInfoChanged).CreateMap();
            //var querySubscription = new Subscription2TaskVoid<int, int>(PageSize, CurrentPage, CalculatePagingSettings).CreateMap();

            _dataSourceService.DataLoadingSettings.OnNext(NavigationStrategyDataLoadingSettings.FetchAll);
        }

        private void DataGridInitializerCallback(bool obj)
        {
            CurrentPage.OnNext(1);
        }

        private Task CalculatePagingSettings(int pageSize, int currentPage)
        {
            _dataSourceService.DataLoadingSettings.OnNext(new NavigationStrategyDataLoadingSettings(skip: (currentPage - 1) * pageSize, take: pageSize));
            return Task.CompletedTask;
        }

        private DataGridPagerInfo PagerInfoChanged(int pagerSize, int pageCount, int currentPage)
        {
            if (currentPage == 0)
                return DataGridPagerInfo.Empty;
            int startPage = (int)Math.Ceiling((decimal)currentPage / (decimal)pagerSize) * pagerSize - pagerSize + 1;
            int endPage = Math.Min(startPage + pagerSize - 1, pageCount);
            return new DataGridPagerInfo(startPage: startPage, endPage: endPage, currentPage: currentPage, totalPages: pageCount);
        }

        public Task Activate()
        {
            //TODO NavigationStrategyBase implementieren und die Funktion virtual markieren
            return Task.CompletedTask;
        }

        public Task Deactivate()
        {
            return Task.CompletedTask;
        }

        public Task NavigateToNextPage()
        {
            int newPage = CurrentPage.Value < PageCount.Value ? CurrentPage.Value + 1 : CurrentPage.Value;
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

        public Task ProcessDataPrequery<T1>(IQueryable<T1> data)
        {
            int items = data.Count();

            int pages = (int)Math.Ceiling(items / (decimal)PageSize.Value);
            PageCount.OnNext(pages);

            int currentPage = CurrentPage.Value == 0 ? 1: Math.Min(pages, CurrentPage.Value);
            
            CurrentPage.OnNext(currentPage);
            return Task.CompletedTask;
        }
    }
}
