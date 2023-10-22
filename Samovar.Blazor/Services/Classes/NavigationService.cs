using System;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public class NavigationService<T>
        : INavigationService
    {
        public BehaviorSubject<DataGridNavigationMode> NavigationMode { get; } = new BehaviorSubject<DataGridNavigationMode>(DataGridNavigationMode.Paging);

        public INavigationStrategy NavigationStrategy { get; private set; }

        public IDataSourceService<T> _dataSourceService { get; }

        private IInitService _initService;
        IVirtualScrollingService _virtualScrollingService;
        private readonly IPagingNavigationStrategy _pagingNavigationStrategy;

        public NavigationService(
              IVirtualScrollingService virtualScrollingService
            , IPagingNavigationStrategy pagingNavigationStrategy
            , IInitService initService
            , IDataSourceService<T> dataSourceService)
        {
            _initService = initService;
            _dataSourceService = dataSourceService;
            _virtualScrollingService = virtualScrollingService;
            _pagingNavigationStrategy = pagingNavigationStrategy;
            
            
            _initService.IsInitialized.Subscribe(DataGridInitializerCallback);
            
             NavigationStrategy = pagingNavigationStrategy;
        }

        private void DataGridInitializerCallback(bool obj)
        {
            //var subscription = new Subscription1TaskVoid<DataGridNavigationMode>(NavigationMode, SetNavigationStrategy).CreateMap();
            //var dataQuerySubscription = new Subscription1TaskVoid<IQueryable<T>>(_dataSourceService.DataQuery, ProcessDataQuery).CreateMap();
            //SetNavigationStrategy(NavigationMode.SubjectValue);
        }

        private Task ProcessDataQuery(IQueryable<T> prequery)
        {
            return NavigationStrategy.ProcessDataPrequery(prequery);
        }

        Task SetNavigationStrategy(DataGridNavigationMode strategy)
        {
            NavigationStrategy?.Deactivate();
            
            NavigationStrategy = strategy switch
            {
                DataGridNavigationMode.Paging => _pagingNavigationStrategy,
                DataGridNavigationMode.VirtualScrolling => _virtualScrollingService,
                _ => throw new NotImplementedException()
            };
            
            NavigationStrategy?.Activate();
            
            //if(_dataSourceService.DataQuery.SubjectValue != null)
            //    NavigationStrategy.ProcessDataPrequery(_dataSourceService.DataQuery.SubjectValue);

            return Task.CompletedTask;
        }
    }
}
