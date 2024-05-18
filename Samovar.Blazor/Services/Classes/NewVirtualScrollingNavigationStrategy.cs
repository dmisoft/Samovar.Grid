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
            //DataLoadingSettings = _dataSourceService.DataQuery.DistinctUntilChanged().Where(x => x != null).Select(x => Task.FromResult(NavigationStrategyDataLoadingSettings.FetchAll));
        }

        //private Task<NavigationStrategyDataLoadingSettings> CalculateDataLoadingSetting(IQueryable<T>? query)
        //{
        //    if(query is null)
        //        return Task.FromResult(NavigationStrategyDataLoadingSettings.Empty);

        //    return Task.FromResult(NavigationStrategyDataLoadingSettings.FetchAll);
        //}
    }
}
