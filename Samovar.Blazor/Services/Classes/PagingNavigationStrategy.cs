using System;
using System.Linq;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    internal class PagingNavigationStrategy<T>
        : IPagingNavigationStrategy
    {
        public ISubject<int> PageSize { get; } = new ParameterSubject<int>(50);

        public ISubject<int> PagerSize { get; } = new ParameterSubject<int>(10);

        public ISubject<int> PageCount { get; set; } = new ParameterSubject<int>(0);

        public ISubject<int> CurrentPage { get; private set; } = new ParameterSubject<int>(0);

        public ISubject<DataGridPagerInfo> PagerInfo { get; private set; } = new ParameterSubject<DataGridPagerInfo>(DataGridPagerInfo.Empty);

        Subscription2<int, int, NavigationStrategyDataLoadingSettings> pagingSettingsSubscription;
        private readonly IInitService _initService;
        private readonly IDataSourceService<T> _dataSourceService;

        public PagingNavigationStrategy(IInitService initService, IDataSourceService<T> dataSourceService)
        {
            _initService = initService;
            _dataSourceService = dataSourceService;
            _initService.IsInitialized.Subscribe(DataGridInitializerCallback);

            PagerInfo = new Subscription3<int, int, int, DataGridPagerInfo>(PagerSize, PageCount, CurrentPage, PagerInfoChanged).CreateMap();
            var querySubscription = new Subscription2TaskVoid<int, int>(PageSize, CurrentPage, CalculatePagingSettings).CreateMap();

            _dataSourceService.DataLoadingSettings.OnNextParameterValue(NavigationStrategyDataLoadingSettings.Empty);
        }

        private void DataGridInitializerCallback(bool obj)
        {
            CurrentPage.OnNextParameterValue(1);
        }

        private Task CalculatePagingSettings(int pageSize, int currentPage)
        {
            _dataSourceService.DataLoadingSettings.OnNextParameterValue(new NavigationStrategyDataLoadingSettings(skip: (currentPage - 1) * pageSize, take: pageSize));
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
            int newPage = CurrentPage.SubjectValue < PageCount.SubjectValue ? CurrentPage.SubjectValue + 1 : CurrentPage.SubjectValue;
            CurrentPage.OnNextParameterValue(newPage);
            return Task.CompletedTask;
        }

        public Task NavigateToPage(int newPage)
        {
            CurrentPage.OnNextParameterValue(newPage);
            return Task.CompletedTask;
        }

        public Task NavigateToPreviousPage()
        {
            int newPage = PagerInfo.SubjectValue.CurrentPage > 1 ? PagerInfo.SubjectValue.CurrentPage - 1 : 1;
            CurrentPage.OnNextParameterValue(newPage);
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

            int pages = (int)Math.Ceiling(items / (decimal)PageSize.SubjectValue);
            PageCount.OnNextParameterValue(pages);

            int currentPage = CurrentPage.SubjectValue == 0 ? 1: Math.Min(pages, CurrentPage.SubjectValue);
            
            CurrentPage.OnNextParameterValue(currentPage);
            return Task.CompletedTask;
        }
    }
}
