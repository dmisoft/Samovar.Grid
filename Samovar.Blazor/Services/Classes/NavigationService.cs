using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public class NavigationService<T>
        : INavigationService
    {
        public BehaviorSubject<DataGridNavigationMode> NavigationMode { get; } = new BehaviorSubject<DataGridNavigationMode>(DataGridNavigationMode.Paging);

        public INavigationStrategy NavigationStrategy { get; private set; }
        private readonly IVirtualScrollingNavigationStrategy _virtualScrollingStrategy;
        private readonly IPagingNavigationStrategy _pagingStrategy;
        private IInitService _initService;

        public NavigationService(
              IVirtualScrollingNavigationStrategy virtualScrollingService
            , IPagingNavigationStrategy pagingNavigationStrategy
            , IInitService initService
            )
        {
            _initService = initService;
            _virtualScrollingStrategy = virtualScrollingService;
            _pagingStrategy = pagingNavigationStrategy;

            _initService.IsInitialized.Subscribe(DataGridInitializerCallback);

            //std. value
            NavigationStrategy = pagingNavigationStrategy;
        }

        private void DataGridInitializerCallback(bool obj)
        {
            //TODO refactoring 10/2023
            //var subscription = new Subscription1TaskVoid<DataGridNavigationMode>(NavigationMode, SetNavigationStrategy).CreateMap();
            //var dataQuerySubscription = new Subscription1TaskVoid<IQueryable<T>>(_dataSourceService.DataQuery, ProcessDataQuery).CreateMap();
            //SetNavigationStrategy(NavigationMode.SubjectValue);

            NavigationMode.Subscribe(SetNavigationStrategy);
            //_dataSourceService.DataQuery.Where(x=>x!=null).Subscribe(ProcessDataQuery);
        }

        //TODO refactoring 10/2023 Task als return value type
        //private void ProcessDataQuery(IQueryable<T> prequery)
        //{
        //    NavigationStrategy.ProcessDataPrequery(prequery);
        //}

        void SetNavigationStrategy(DataGridNavigationMode strategy)
        {
            NavigationStrategy = strategy switch
            {
                DataGridNavigationMode.Paging => _pagingStrategy,
                DataGridNavigationMode.VirtualScrolling => _virtualScrollingStrategy,
                _ => throw new NotImplementedException()
            };
        }
    }
}
