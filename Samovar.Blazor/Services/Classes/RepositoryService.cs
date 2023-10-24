using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public class RepositoryService<T>
        : IRepositoryService<T>
    {
        public IEnumerable<SmDataGridRowModel<T>> ViewCollection { get; private set; }
        public IObservable<IEnumerable<SmDataGridRowModel<T>>> ViewCollectionObservable { get; private set; }

        private readonly IDataSourceService<T> _dataSourceService;
        INavigationService _navigationService;
        IColumnService _columnService;
        IInitService _initService;
        IRowDetailService<T> _rowDetailService;
        private bool repositoryForVirtualScrollingInitialized;
        private readonly IGridStateService _stateService;
        private readonly ILayoutService _layoutService;

        public BehaviorSubject<HashSet<T>> Data { get; private set; } = new BehaviorSubject<HashSet<T>>(new HashSet<T>());

        public Dictionary<string, PropertyInfo> PropInfo { get; } = new Dictionary<string, PropertyInfo>();
        //public static Dictionary<string, PropertyInfo> PropInfoStatic { get; } = new Dictionary<string, PropertyInfo>();
        public static Dictionary<string, Func<T, int>> PropInfoDelegateInt { get; } = new Dictionary<string, Func<T, int>>();
        public static Dictionary<string, Func<T, string>> PropInfoDelegateString { get; } = new Dictionary<string, Func<T, string>>();
        public static Dictionary<string, Func<T, DateTime>> PropInfoDelegateDate { get; } = new Dictionary<string, Func<T, DateTime>>();

        public RepositoryService(
              IDataSourceService<T> dataSourceService
            , INavigationService navigationService
            , IColumnService columnService
            , IInitService initService
            , IRowDetailService<T> rowDetailService
            , IGridStateService stateService
            , ILayoutService layoutService)
        {
            _dataSourceService = dataSourceService;
            _navigationService = navigationService;
            _columnService = columnService;
            _initService = initService;
            _rowDetailService = rowDetailService;
            _stateService = stateService;
            _layoutService = layoutService;

            Type t = typeof(T);

            foreach (PropertyInfo pi in typeof(T).GetProperties())
            {
                PropInfo.Add(pi.Name, pi);
                //pi.PropertyType
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

            _initService.IsInitialized.Subscribe(DataGridInitializerCallback);
        }

        private void DataGridInitializerCallback(bool obj)
        {
            //TODO refactoring 10/2023
            //var querySubscription = new Subscription2TaskVoid<IQueryable<T>, NavigationStrategyDataLoadingSettings>(_dataSourceService.DataQuery, _dataSourceService.DataLoadingSettings, DataLoadingSettingsCallback2).CreateMap();
            ViewCollectionObservable = Observable.CombineLatest(
                _dataSourceService.DataQuery,
                _dataSourceService.DataLoadingSettings,
                ViewCollectionObservableMap
            );
            ViewCollectionObservable.Subscribe(dummy);
            //_dataSourceService.DataQuery.Subscribe(dataQueryObserver);
            //_dataSourceService.DataLoadingSettings.Subscribe(dataLoadingSettingsObserver);
        }

        private void dummy(IEnumerable<SmDataGridRowModel<T>> enumerable)
        {
            //throw new NotImplementedException();
        }

        //bundle data query and loading settngs to common observable row model collection
        private IEnumerable<SmDataGridRowModel<T>> ViewCollectionObservableMap(IQueryable<T> query, NavigationStrategyDataLoadingSettings loadingSettings)
        {
            if (query == null)
            {
                _stateService.DataSourceState.OnNext(DataSourceStateEnum.NoData);
                return null;
            }

            query = query.Skip(loadingSettings.Skip).Take(loadingSettings.Take);

            if (_navigationService.NavigationMode.Value == DataGridNavigationMode.Paging)
            {
                _stateService.DataSourceState.OnNext(DataSourceStateEnum.Loading);
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                ViewCollection = CreateRowModelList(query, _columnService.DataColumnModels, PropInfo);
                stopWatch.Stop();

                if (ViewCollection.Count() == 0)
                {
                    _stateService.DataSourceState.OnNext(DataSourceStateEnum.NoData);
                }
                else
                {
                    _stateService.DataSourceState.OnNext(DataSourceStateEnum.Idle);
                }
            }
            else if (_navigationService.NavigationMode.Value == DataGridNavigationMode.VirtualScrolling && !repositoryForVirtualScrollingInitialized)
            {
                _stateService.DataSourceState.OnNext(DataSourceStateEnum.Idle);
                repositoryForVirtualScrollingInitialized = true;
            }
            else if (_navigationService.NavigationMode.Value == DataGridNavigationMode.VirtualScrolling && repositoryForVirtualScrollingInitialized)
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                ViewCollection = CreateRowModelList(query, _columnService.DataColumnModels, PropInfo);

                stopWatch.Stop();
                // Get the elapsed time as a TimeSpan value.
                TimeSpan ts = stopWatch.Elapsed;

                // Format and display the TimeSpan value.
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                Console.WriteLine("RunTime " + elapsedTime);

                if (ViewCollection.Count() == 0)
                {
                    _stateService.DataSourceState.OnNext(DataSourceStateEnum.NoData);
                }
                else
                {
                    //TODO extra Idle state for virtual scrolling???
                    _stateService.DataSourceState.OnNext(DataSourceStateEnum.Idle);
                }
            }

            //await OnViewCollectionChanged(ViewCollection);

            return ViewCollection;
        }

        //public event Func<IEnumerable<SmDataGridRowModel<T>>, Task> ViewCollectionChanged;
        //internal async Task OnViewCollectionChanged(IEnumerable<SmDataGridRowModel<T>> data)
        //{
        //    if (ViewCollectionChanged != null)
        //    {
        //        await ViewCollectionChanged.Invoke(data);
        //    }
        //}

        private List<SmDataGridRowModel<T>> CreateRowModelList(IQueryable<T> gridData, IEnumerable<IDataColumnModel> ColumnMetadataList, Dictionary<string, PropertyInfo> PropInfo)
        {
            var retVal = new List<SmDataGridRowModel<T>>();
            int rowPosition = 0;

            try
            {
                foreach (var keyDataPair in gridData.ToHashSet())
                {
                    rowPosition++;
                    retVal.Add(new SmDataGridRowModel<T>(keyDataPair, ColumnMetadataList, rowPosition, PropInfo, _rowDetailService.ExpandedRowDetails.Value.Any(r => r.Equals(keyDataPair))));
                    Task.Delay(100).Wait();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            //await Task.Run(() =>
            //{
            //    int rowPosition = 0;

            //    try
            //    {
            //        foreach (var keyDataPair in gridData.ToHashSet())
            //        {
            //            rowPosition++;
            //            retVal.Add(new SmDataGridRowModel<T>(keyDataPair, ColumnMetadataList, rowPosition, PropInfo, _rowDetailService.ExpandedRowDetails.SubjectValue.Any(r => r.Equals(keyDataPair))));
            //            //await Task.Delay(5);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        throw;
            //    }

            //});

            return retVal;
        }

        public void AttachViewCollectionSubscription()
        {
            //ViewCollectionSubscription.Attach();
            //TotalItemsCountSubscription.Attach();
        }

        public void DetachViewCollectionSubscription()
        {
            //ViewCollectionSubscription.Detach();
            //TotalItemsCountSubscription.Detach();
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
