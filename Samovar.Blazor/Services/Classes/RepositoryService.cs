using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;

namespace Samovar.Blazor;

public class RepositoryService<T>
    : IRepositoryService<T>, IAsyncDisposable
{
    public IEnumerable<GridRowModel<T>> ViewCollection { get; } = new List<GridRowModel<T>>();

    private readonly IDataSourceService<T> _dataSourceService;
    private readonly INavigationService _navigationService;
    private readonly IColumnService _columnService;
    private readonly IRowDetailService<T> _rowDetailService;
    private readonly IGridStateService _stateService;

    public BehaviorSubject<HashSet<T>> Data { get; private set; } = new BehaviorSubject<HashSet<T>>(new HashSet<T>());

    public Dictionary<string, PropertyInfo> PropInfo { get; } = new Dictionary<string, PropertyInfo>();
    public static Dictionary<string, Func<T, int>> PropInfoDelegateInt { get; } = new Dictionary<string, Func<T, int>>();
    public static Dictionary<string, Func<T, string>> PropInfoDelegateString { get; } = new Dictionary<string, Func<T, string>>();
    public static Dictionary<string, Func<T, DateTime>> PropInfoDelegateDate { get; } = new Dictionary<string, Func<T, DateTime>>();
    public IObservable<Task<IEnumerable<GridRowModel<T>>>> ViewCollectionObservableTask { get; set; }

    public RepositoryService(
          IDataSourceService<T> dataSourceService
        , INavigationService navigationService
        , IColumnService columnService
        , IRowDetailService<T> rowDetailService
        , IGridStateService stateService)
    {
        _dataSourceService = dataSourceService;
        _navigationService = navigationService;
        _columnService = columnService;
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

        ViewCollectionObservableTask = Observable.CombineLatest(
            _dataSourceService.DataQuery,
            _navigationService.NavigationStrategy.DataLoadingSettings,
            ViewCollectionObservableMap);

        _navigationService.NavigationMode.Subscribe(s => {
            ViewCollectionObservableTask = Observable.CombineLatest(
            _dataSourceService.DataQuery,
            _navigationService.NavigationStrategy.DataLoadingSettings,
            ViewCollectionObservableMap);
        });
    }

    private Task<IEnumerable<GridRowModel<T>>> ViewCollectionObservableMap(IQueryable<T>? query, NavigationStrategyDataLoadingSettings navigationStrategyDataLoadingSettings)
    {
        Debug.WriteLine("ViewCollectionObservableMap");
        _stateService.DataSourceState.OnNext(Task.FromResult(DataSourceState.Loading));

        if (query is null)
        {
            _stateService.DataSourceState.OnNext(Task.FromResult(DataSourceState.NoData));
            return Task.FromResult(new List<GridRowModel<T>>().AsEnumerable());
        }

        IEnumerable<GridRowModel<T>> _retVal;

        if (!navigationStrategyDataLoadingSettings.ShowAll)
            query = query.Skip(navigationStrategyDataLoadingSettings.Skip).Take(navigationStrategyDataLoadingSettings.Take);

        _retVal = CreateRowModelList(query, _columnService.DataColumnModels, PropInfo);

        if(_retVal.Any())
            _stateService.DataSourceState.OnNext(Task.FromResult(DataSourceState.Idle));
        else
            _stateService.DataSourceState.OnNext(Task.FromResult(DataSourceState.NoData));
        
        return Task.FromResult(_retVal);
    }


    private List<GridRowModel<T>> CreateRowModelList(IQueryable<T> gridData, IEnumerable<IDataColumnModel> ColumnMetadataList, Dictionary<string, PropertyInfo> PropInfo)
    {
        var retVal = new List<GridRowModel<T>>();
        int rowPosition = 0;

        foreach (var keyDataPair in gridData.ToHashSet())
        {
            rowPosition++;
            retVal.Add(new GridRowModel<T>(keyDataPair, ColumnMetadataList, rowPosition, PropInfo, _rowDetailService.ExpandedRowDetails.Value.Any(r => r!.Equals(keyDataPair))));
        }

        return retVal;
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}


