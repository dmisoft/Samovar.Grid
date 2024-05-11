using Microsoft.AspNetCore.Components;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;

namespace Samovar.Blazor
{
    public class RepositoryService<T>
        : IRepositoryService<T>, IAsyncDisposable
    {
        private IDisposable? _viewCollectionObservableTaskSubscription;

        public IEnumerable<SmDataGridRowModel<T>> ViewCollection { get; } = new List<SmDataGridRowModel<T>>();

        private readonly IDataSourceService<T> _dataSourceService;
        private readonly INavigationService _navigationService;
        private readonly IColumnService _columnService;
        private readonly IRowDetailService<T> _rowDetailService;
        private readonly IGridStateService _stateService;
        private readonly IInitService _initService;

        public BehaviorSubject<HashSet<T>> Data { get; private set; } = new BehaviorSubject<HashSet<T>>(new HashSet<T>());

        public Dictionary<string, PropertyInfo> PropInfo { get; } = new Dictionary<string, PropertyInfo>();
        public static Dictionary<string, Func<T, int>> PropInfoDelegateInt { get; } = new Dictionary<string, Func<T, int>>();
        public static Dictionary<string, Func<T, string>> PropInfoDelegateString { get; } = new Dictionary<string, Func<T, string>>();
        public static Dictionary<string, Func<T, DateTime>> PropInfoDelegateDate { get; } = new Dictionary<string, Func<T, DateTime>>();
        public List<EventCallback<IEnumerable<SmDataGridRowModel<T>>>> CollectionViewChangedEvList { get; set; } = new List<EventCallback<IEnumerable<SmDataGridRowModel<T>>>>();

        public RepositoryService(
              IDataSourceService<T> dataSourceService
            , INavigationService navigationService
            , IColumnService columnService
            , IInitService initService
            , IRowDetailService<T> rowDetailService
            , IGridStateService stateService)
        {
            _dataSourceService = dataSourceService;
            _navigationService = navigationService;
            _columnService = columnService;
            _initService = initService;
            _rowDetailService = rowDetailService;
            _stateService = stateService;

            foreach (PropertyInfo pi in typeof(T).GetProperties())
            {
                PropInfo.Add(pi.Name, pi);

                switch (pi.PropertyType)
                {
                    case var ts when ts == typeof(string):
                        if (!PropInfoDelegateString.ContainsKey(pi.Name))
                            PropInfoDelegateString.Add(pi.Name, (Func<T, string>)Delegate.CreateDelegate(typeof(Func<T, string>), pi.GetGetMethod(true)!));
                        break;
                    case var ts when ts == typeof(DateTime) || ts == typeof(DateTime?):
                        if (!PropInfoDelegateDate.ContainsKey(pi.Name))
                            PropInfoDelegateDate.Add(pi.Name, (Func<T, DateTime>)Delegate.CreateDelegate(typeof(Func<T, DateTime>), pi.GetGetMethod(true)!));
                        break;
                    case var ts when ts == typeof(int):
                        if (!PropInfoDelegateInt.ContainsKey(pi.Name))
                            PropInfoDelegateInt.Add(pi.Name, (Func<T, int>)Delegate.CreateDelegate(typeof(Func<T, int>), pi.GetGetMethod(true)!));
                        break;
                    default:
                        break;
                }
            }

            SubscribeInitializing();
        }

        private void SubscribeInitializing()
        {
            _initService.IsInitialized.Subscribe(DataGridInitializerCallback);
        }


        private void DataGridInitializerCallback(bool obj)
        {
            IObservable<Task<IEnumerable<SmDataGridRowModel<T>>>> ViewCollectionObservableTask = Observable.CombineLatest(
            _dataSourceService.DataQuery,
            _navigationService.NavigationStrategy.DataLoadingSettings,
            ViewCollectionObservableMap11);

            _viewCollectionObservableTaskSubscription = ViewCollectionObservableTask.Subscribe(async getNewCollectionViewTask =>
            {
                try
                {
                    var newCollectionView = await getNewCollectionViewTask;

                    if (!newCollectionView.Any())
                    {
                        _stateService.DataSourceState.OnNext(DataSourceState.NoData);
                        _stateService.DataSourceStateEvList.ForEach(x => x.InvokeAsync(DataSourceState.NoData));
                    }
                    else
                    {
                        _stateService.DataSourceState.OnNext(DataSourceState.Idle);
                        _stateService.DataSourceStateEvList.ForEach(x => x.InvokeAsync(DataSourceState.Idle));
                    }
                    CollectionViewChangedEvList.ForEach(x => x.InvokeAsync(newCollectionView));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Task failed with error: {ex.Message}");
                }
            });
        }

        private async Task<IEnumerable<SmDataGridRowModel<T>>> ViewCollectionObservableMap11(IQueryable<T>? query, Task<NavigationStrategyDataLoadingSettings> loadingSettingsTask)
        {
            if (query is null)
                return new List<SmDataGridRowModel<T>>();

            IEnumerable<SmDataGridRowModel<T>> _retVal;
            var loadingSettings = await loadingSettingsTask;

            query = query.Skip(loadingSettings.Skip).Take(loadingSettings.Take);

            _stateService.DataSourceState.OnNext(DataSourceState.Loading);
            _stateService.DataSourceStateEvList.ForEach(x => x.InvokeAsync(DataSourceState.Loading));

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            _retVal = CreateRowModelList(query, _columnService.DataColumnModels, PropInfo);
            stopWatch.Stop();

            return _retVal;
        }


        private List<SmDataGridRowModel<T>> CreateRowModelList(IQueryable<T> gridData, IEnumerable<IDataColumnModel> ColumnMetadataList, Dictionary<string, PropertyInfo> PropInfo)
        {
            var retVal = new List<SmDataGridRowModel<T>>();
            int rowPosition = 0;

            foreach (var keyDataPair in gridData.ToHashSet())
            {
                rowPosition++;
                retVal.Add(new SmDataGridRowModel<T>(keyDataPair, ColumnMetadataList, rowPosition, PropInfo, _rowDetailService.ExpandedRowDetails.Value.Any(r => r!.Equals(keyDataPair))));
            }

            return retVal;
        }

        public ValueTask DisposeAsync()
        {
            _viewCollectionObservableTaskSubscription?.Dispose();
            return ValueTask.CompletedTask;
        }
    }

    static class MyLocalExtension
    {
        public static Func<T1, TResult> CreatePropertyOrFieldReaderDelegate<T1, TResult>(string field)
        {
            var input = Expression.Parameter(typeof(T1));
            return Expression.Lambda<Func<T1, TResult>>(Expression.PropertyOrField(input, field), input).Compile();
        }
        static public Func<S, T1> CreateGetPropertyDelegate<S, T1>(this PropertyInfo propInfo)
        {
            var instExp = Expression.Parameter(typeof(S));
            var fieldExp = Expression.Property(instExp, propInfo);
            return Expression.Lambda<Func<S, T1>>(fieldExp, instExp).Compile();
        }
    }
}
