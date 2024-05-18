using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    internal class NewVirtualScrollingNavigationStrategy<T>
        : INewVirtualScrollingNavigationStrategy
    {
        public required BehaviorSubject<NavigationStrategyDataLoadingSettings> DataLoadingSettings { get; set; } = new BehaviorSubject<NavigationStrategyDataLoadingSettings>(NavigationStrategyDataLoadingSettings.FetchAll);

        private readonly IDataSourceService<T> _dataSourceService;

        public NewVirtualScrollingNavigationStrategy(
              IInitService initService
            , IDataSourceService<T> dataSourceService)
        {
            _dataSourceService = dataSourceService;
            
            initService.IsInitialized.Subscribe(DataGridInitializerCallback);
        }

        private void DataGridInitializerCallback(bool obj)
        {
            _dataSourceService.DataQuery.DistinctUntilChanged().Where(x => x != null)
                .Subscribe(x => {
                    DataLoadingSettings.OnNext(NavigationStrategyDataLoadingSettings.FetchAll);
                });
        }
    }
}
