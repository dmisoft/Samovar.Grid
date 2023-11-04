using Microsoft.AspNetCore.Components;
using System;
using System.Collections;
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
        //public BehaviorSubject<IEnumerable<SmDataGridRowModel<T>>> ViewCollectionObservable { get; private set; } = new BehaviorSubject<IEnumerable<SmDataGridRowModel<T>>>(new List<SmDataGridRowModel<T>>());
        //public Func<IEnumerable<SmDataGridRowModel<T>>, Task> ViewCollectionChanged { get; set; }

        private IObservable<Task<IEnumerable<SmDataGridRowModel<T>>>> ViewCollectionObservableTask;
        private IObservable<IEnumerable<SmDataGridRowModel<T>>> ViewCollectionObservableIntern;

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
        public List<EventCallback<IEnumerable<SmDataGridRowModel<T>>>> CollectionViewChangedEvList { get; set; } = new List<EventCallback<IEnumerable<SmDataGridRowModel<T>>>>();

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
            //ViewCollectionObservableIntern = Observable.CombineLatest(
            //    _dataSourceService.DataQuery,
            //    _navigationService.NavigationStrategy.DataLoadingSettings,
            //    ViewCollectionObservableMap
            //);
            //ViewCollectionObservableIntern.Subscribe(dummydummy);

            //IObservable<Task<int>> taskObservable = Observable.FromAsync((val) => SomeAsyncMethod(val));

            ViewCollectionObservableTask = Observable.CombineLatest(
                _dataSourceService.DataQuery,
                _navigationService.NavigationStrategy.DataLoadingSettings,
                //async (data, loadingSettings) => await ViewCollectionObservableMap11(data,loadingSettings)
                ViewCollectionObservableMap11
            );

            IDisposable ViewCollectionObservableTaskSubscription = ViewCollectionObservableTask.Subscribe(async task =>
            {
                try
                {
                    var newCollectionView = await task;

                    if (newCollectionView.Count() == 0)
                    {
                        _stateService.DataSourceState.OnNext(DataSourceStateEnum.NoData);
                        _stateService.DataSourceStateEvList.ForEach(x => x.InvokeAsync(DataSourceStateEnum.NoData));
                    }
                    else
                    {
                        _stateService.DataSourceState.OnNext(DataSourceStateEnum.Idle);
                        _stateService.DataSourceStateEvList.ForEach(x => x.InvokeAsync(DataSourceStateEnum.Idle));
                    }
                    CollectionViewChangedEvList.ForEach(x => x.InvokeAsync(newCollectionView));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Task failed with error: {ex.Message}");
                }
            });

            //IObservable<Task<IEnumerable<SmDataGridRowModel<T>>>> taskObservable = _dataSourceService.DataQuery.Select(item => SomeAsyncMethod(item));
            
            //IDisposable subscription = taskObservable.Subscribe(async task =>
            //{
            //    try
            //    {
            //        var result = await task;
            //        Console.WriteLine($"Task completed with result: {result}");
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"Task failed with error: {ex.Message}");
            //    }
            //});
        }

        private async Task<IEnumerable<SmDataGridRowModel<T>>> ViewCollectionObservableMap11(IQueryable<T> query, NavigationStrategyDataLoadingSettings loadingSettings)
        {
            //await Task.Run(async () => {
            //    await Task.Delay(1);
            //});
            IEnumerable<SmDataGridRowModel<T>> _retVal = null;
            await Task.Delay(1);
            //if (query == null)
            //{
            //    _stateService.DataSourceState.OnNext(DataSourceStateEnum.NoData);
            //    _stateService.DataSourceStateEvList.ForEach(x => x.InvokeAsync(DataSourceStateEnum.NoData));
            //}
            query = query.Skip(loadingSettings.Skip).Take(loadingSettings.Take);

            if (_navigationService.NavigationMode.Value == DataGridNavigationMode.Paging)
            {
                _stateService.DataSourceState.OnNext(DataSourceStateEnum.Loading);
                _stateService.DataSourceStateEvList.ForEach(x => x.InvokeAsync(DataSourceStateEnum.Loading));

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                _retVal = CreateRowModelList(query, _columnService.DataColumnModels, PropInfo);
                stopWatch.Stop();
            }

            return _retVal;
        }

        private Task<IEnumerable<SmDataGridRowModel<T>>> SomeAsyncMethod(IQueryable<T> query)
        {
            return Task.FromResult(new List<SmDataGridRowModel<T>>().AsEnumerable());
            //Task.Delay(1);
            //return Task.FromResult(query.ToList().AsEnumerable());
        }

        private void test1(object obj)
        {
            
        }

        private void test(NavigationStrategyDataLoadingSettings settings)
        {
            throw new NotImplementedException();
        }

        private void dummydummy(IEnumerable<SmDataGridRowModel<T>> enumerable)
        {
            
        } 

        private void viewCollectionObserverHandler(IEnumerable<SmDataGridRowModel<T>> enumerable)
        {
            if (enumerable.Count() == 0)
            {
                _stateService.DataSourceState.OnNext(DataSourceStateEnum.NoData);
                //_stateService.DataSourceStateEv.InvokeAsync(DataSourceStateEnum.NoData);
                _stateService.DataSourceStateEvList.ForEach(x => x.InvokeAsync(DataSourceStateEnum.NoData));
            }
            else
            {
                _stateService.DataSourceState.OnNext(DataSourceStateEnum.Idle);
                //_stateService.DataSourceStateEv.InvokeAsync(DataSourceStateEnum.Idle);
                _stateService.DataSourceStateEvList.ForEach(x => x.InvokeAsync(DataSourceStateEnum.Idle));
                //ViewCollectionChanged?.Invoke(enumerable);
                //ViewCollectionObservable.OnNext(enumerable);
            }
            CollectionViewChangedEvList.ForEach(x => x.InvokeAsync(enumerable));

            // Dispose of the subscription when you're done.
            viewCollectionObserverSubscription?.Dispose();
        }

        //private Task<IEnumerable<SmDataGridRowModel<T>>> ViewCollectionObservableMap11(IQueryable<T> query, NavigationStrategyDataLoadingSettings loadingSettings)
        //{
        //    return Task.Run(async () => {
        //        await Task.Delay(1);
        //        if (query == null)
        //        {
        //            _stateService.DataSourceState.OnNext(DataSourceStateEnum.NoData);
        //            await _stateService.DataSourceStateEv.InvokeAsync(DataSourceStateEnum.NoData);
        //            return null;
        //        }

        //        query = query.Skip(loadingSettings.Skip).Take(loadingSettings.Take);
                
        //        if (_navigationService.NavigationMode.Value == DataGridNavigationMode.Paging)
        //        {
        //            _stateService.DataSourceState.OnNext(DataSourceStateEnum.Loading);
        //            await _stateService.DataSourceStateEv.InvokeAsync(DataSourceStateEnum.Loading);

        //            Stopwatch stopWatch = new Stopwatch();
        //            stopWatch.Start();
        //            ViewCollection = CreateRowModelList(query, _columnService.DataColumnModels, PropInfo);
        //            stopWatch.Stop();

        //            if (ViewCollection.Count() == 0)
        //            {
        //                _stateService.DataSourceState.OnNext(DataSourceStateEnum.NoData);
        //            }
        //            else
        //            {
        //                _stateService.DataSourceState.OnNext(DataSourceStateEnum.Idle);
        //            }
        //        }
        //        else if (_navigationService.NavigationMode.Value == DataGridNavigationMode.VirtualScrolling && !repositoryForVirtualScrollingInitialized)
        //        {
        //            _stateService.DataSourceState.OnNext(DataSourceStateEnum.Idle);
        //            repositoryForVirtualScrollingInitialized = true;
        //        }
        //        else if (_navigationService.NavigationMode.Value == DataGridNavigationMode.VirtualScrolling && repositoryForVirtualScrollingInitialized)
        //        {
        //            Stopwatch stopWatch = new Stopwatch();
        //            stopWatch.Start();

        //            ViewCollection = CreateRowModelList(query, _columnService.DataColumnModels, PropInfo);

        //            stopWatch.Stop();
        //            // Get the elapsed time as a TimeSpan value.
        //            TimeSpan ts = stopWatch.Elapsed;

        //            // Format and display the TimeSpan value.
        //            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
        //                ts.Hours, ts.Minutes, ts.Seconds,
        //                ts.Milliseconds / 10);
        //            Console.WriteLine("RunTime " + elapsedTime);

        //            if (ViewCollection.Count() == 0)
        //            {
        //                _stateService.DataSourceState.OnNext(DataSourceStateEnum.NoData);
        //            }
        //            else
        //            {
        //                //TODO extra Idle state for virtual scrolling???
        //                _stateService.DataSourceState.OnNext(DataSourceStateEnum.Idle);
        //            }
                    
        //        }
        //        //ViewCollectionObservable = Observable.Create<IEnumerable<SmDataGridRowModel<T>>>(observer => {
        //        //    observer.OnNext(ViewCollection);
        //        //    observer.OnCompleted();
        //        //    return Disposable.Empty;
        //        //});
        //        //ViewCollectionObservable = ViewCollection.ToObservable().Subscribe(hohoho);
        //        //ViewCollectionObservable.OnNext(ViewCollection);

        //        return ViewCollection;
        //    });
        //    //return Task.FromResult(ViewCollection);

        //}
        
        IDisposable viewCollectionObserverSubscription;
        IObservable<IEnumerable<SmDataGridRowModel<T>>> customObservable;
        private IEnumerable<SmDataGridRowModel<T>> ViewCollectionObservableMap(IQueryable<T> query, NavigationStrategyDataLoadingSettings loadingSettings)
        {
            //if (query == null)
            //{
            //    _stateService.DataSourceState.OnNext(DataSourceStateEnum.NoData);
            //    //await _stateService.DataSourceStateEv.InvokeAsync(DataSourceStateEnum.NoData);
            //    _stateService.DataSourceStateEvList.ForEach(x => x.InvokeAsync(DataSourceStateEnum.NoData));
            //    //CollectionViewChangedEvList.ForEach(x => x.InvokeAsync(null));

            //    return null;;
            //}

            //query = query.Skip(loadingSettings.Skip).Take(loadingSettings.Take);

            //if (_navigationService.NavigationMode.Value == DataGridNavigationMode.Paging)
            //{
            //    _stateService.DataSourceState.OnNext(DataSourceStateEnum.Loading);
            //    //await _stateService.DataSourceStateEv.InvokeAsync(DataSourceStateEnum.Loading);
            //    _stateService.DataSourceStateEvList.ForEach(x => x.InvokeAsync(DataSourceStateEnum.Loading));

            //    Stopwatch stopWatch = new Stopwatch();
            //    stopWatch.Start();
            //    ViewCollection = CreateRowModelList(query, _columnService.DataColumnModels, PropInfo);
            //    stopWatch.Stop();
            //    //CollectionViewChangedEvList.ForEach(x => x.InvokeAsync(ViewCollection));

            //}
            //return ViewCollection;

            customObservable = Observable.Create<IEnumerable<SmDataGridRowModel<T>>>(async (observer) =>
            {
                await Task.Run(async () => {
                    await Task.Delay(1);
                });
                if (query == null)
                {
                    _stateService.DataSourceState.OnNext(DataSourceStateEnum.NoData);
                    //await _stateService.DataSourceStateEv.InvokeAsync(DataSourceStateEnum.NoData);
                    _stateService.DataSourceStateEvList.ForEach(x => x.InvokeAsync(DataSourceStateEnum.NoData));
                    //CollectionViewChangedEvList.ForEach(x => x.InvokeAsync(null));

                    observer.OnNext(null);
                }

                query = query.Skip(loadingSettings.Skip).Take(loadingSettings.Take);

                if (_navigationService.NavigationMode.Value == DataGridNavigationMode.Paging)
                {
                    _stateService.DataSourceState.OnNext(DataSourceStateEnum.Loading);
                    //await _stateService.DataSourceStateEv.InvokeAsync(DataSourceStateEnum.Loading);
                    _stateService.DataSourceStateEvList.ForEach(x => x.InvokeAsync(DataSourceStateEnum.Loading));

                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    ViewCollection = CreateRowModelList(query, _columnService.DataColumnModels, PropInfo);
                    stopWatch.Stop();
                    //CollectionViewChangedEvList.ForEach(x => x.InvokeAsync(ViewCollection));

                }
                //else if (_navigationService.NavigationMode.Value == DataGridNavigationMode.VirtualScrolling && !repositoryForVirtualScrollingInitialized)
                //{
                //    _stateService.DataSourceState.OnNext(DataSourceStateEnum.Idle);
                //    repositoryForVirtualScrollingInitialized = true;
                //}
                //else if (_navigationService.NavigationMode.Value == DataGridNavigationMode.VirtualScrolling && repositoryForVirtualScrollingInitialized)
                //{
                //    Stopwatch stopWatch = new Stopwatch();
                //    stopWatch.Start();

                //    ViewCollection = CreateRowModelList(query, _columnService.DataColumnModels, PropInfo);

                //    stopWatch.Stop();
                //    // Get the elapsed time as a TimeSpan value.
                //    TimeSpan ts = stopWatch.Elapsed;

                //    // Format and display the TimeSpan value.
                //    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                //        ts.Hours, ts.Minutes, ts.Seconds,
                //        ts.Milliseconds / 10);
                //    Console.WriteLine("RunTime " + elapsedTime);

                //    if (ViewCollection.Count() == 0)
                //    {
                //        _stateService.DataSourceState.OnNext(DataSourceStateEnum.NoData);
                //    }
                //    else
                //    {
                //        //TODO extra Idle state for virtual scrolling???
                //        _stateService.DataSourceState.OnNext(DataSourceStateEnum.Idle);
                //    }
                //}
                observer.OnNext(ViewCollection);
                observer.OnCompleted();
                //customObservable.Subscribe(viewCollectionObserverHandler);
            });

            // Subscribe to the custom observable.
            //IDisposable subscription = customObservable.Subscribe(
            //    value => ViewCollectionObservable.OnNext(value),
            //    error => Console.WriteLine($"Error: {error.Message}"),
            //    () => Console.WriteLine("Observable completed")
            //);
            viewCollectionObserverSubscription = customObservable.Subscribe(viewCollectionObserverHandler);

            // Dispose of the subscription when you're done.
            //subscription.Dispose();

            return new List<SmDataGridRowModel<T>>();
        }

        //bundle data query and loading settngs to common observable row model collection
        //private IEnumerable<SmDataGridRowModel<T>> ViewCollectionObservableMapAsync(IQueryable<T> query, NavigationStrategyDataLoadingSettings loadingSettings)
        //{
        //    //if (query == null)
        //    //{
        //    //    _stateService.DataSourceState.OnNext(DataSourceStateEnum.NoData);
        //    //    //_stateService.DataSourceState.OnCompleted();
        //    //    return null;
        //    //}

        //    //query = query.Skip(loadingSettings.Skip).Take(loadingSettings.Take);

        //    //if (_navigationService.NavigationMode.Value == DataGridNavigationMode.Paging)
        //    //{
        //    //    _stateService.DataSourceState.OnNext(DataSourceStateEnum.Loading);
        //    //    //_stateService.DataSourceState.OnCompleted();
        //    //    //Stopwatch stopWatch = new Stopwatch();
        //    //    //stopWatch.Start();

        //    //    ViewCollection = CreateRowModelList(query, _columnService.DataColumnModels, PropInfo);

        //    //    //stopWatch.Stop();

        //    //    //if (ViewCollection.Count() == 0)
        //    //    //{
        //    //    //    ViewCollectionObservable.OnNext(ViewCollection);
        //    //    //    ViewCollectionObservable.OnCompleted();
        //    //    //    _stateService.DataSourceState.OnNext(DataSourceStateEnum.NoData);
        //    //    //    _stateService.DataSourceState.OnCompleted();
        //    //    //}
        //    //    //else
        //    //    //{
        //    //    //    ViewCollectionObservable.OnNext(ViewCollection);
        //    //    //    ViewCollectionObservable.OnCompleted();
        //    //    //    _stateService.DataSourceState.OnNext(DataSourceStateEnum.Idle);
        //    //    //    _stateService.DataSourceState.OnCompleted();
        //    //    //}
        //    //}
        //    //return ViewCollection;
            
        //    Task.Run(async () =>
        //    {
        //        await Task.Delay(1);
        //        if (query == null)
        //        {
        //            _stateService.DataSourceState.OnNext(DataSourceStateEnum.NoData);
        //            return;
        //        }

        //        query = query.Skip(loadingSettings.Skip).Take(loadingSettings.Take);

        //        if (_navigationService.NavigationMode.Value == DataGridNavigationMode.Paging)
        //        {
        //            _stateService.DataSourceState.OnNext(DataSourceStateEnum.Loading);
        //            //Stopwatch stopWatch = new Stopwatch();
        //            //stopWatch.Start();

        //            ViewCollection = CreateRowModelList(query, _columnService.DataColumnModels, PropInfo);
        //            //stopWatch.Stop();

        //            if (ViewCollection.Count() == 0)
        //            {
                        
        //                _stateService.DataSourceState.OnNext(DataSourceStateEnum.NoData);
                        
        //            }
        //            else
        //            {
        //                _stateService.DataSourceState.OnNext(DataSourceStateEnum.Idle);
        //                ViewCollectionObservable.OnNext(ViewCollection);
        //            }
        //        }
        //        //else if (_navigationService.NavigationMode.Value == DataGridNavigationMode.VirtualScrolling && !repositoryForVirtualScrollingInitialized)
        //        //{
        //        //    _stateService.DataSourceState.OnNext(DataSourceStateEnum.Idle);
        //        //    repositoryForVirtualScrollingInitialized = true;
        //        //}
        //        //else if (_navigationService.NavigationMode.Value == DataGridNavigationMode.VirtualScrolling && repositoryForVirtualScrollingInitialized)
        //        //{
        //        //    Stopwatch stopWatch = new Stopwatch();
        //        //    stopWatch.Start();

        //        //    ViewCollection = CreateRowModelList(query, _columnService.DataColumnModels, PropInfo);

        //        //    stopWatch.Stop();
        //        //    // Get the elapsed time as a TimeSpan value.
        //        //    TimeSpan ts = stopWatch.Elapsed;

        //        //    // Format and display the TimeSpan value.
        //        //    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
        //        //        ts.Hours, ts.Minutes, ts.Seconds,
        //        //        ts.Milliseconds / 10);
        //        //    Console.WriteLine("RunTime " + elapsedTime);

        //        //    if (ViewCollection.Count() == 0)
        //        //    {
        //        //        _stateService.DataSourceState.OnNext(DataSourceStateEnum.NoData);
        //        //    }
        //        //    else
        //        //    {
        //        //        //TODO extra Idle state for virtual scrolling???
        //        //        _stateService.DataSourceState.OnNext(DataSourceStateEnum.Idle);
        //        //    }
        //        //}

        //    });
        //    return new List<SmDataGridRowModel<T>>();
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
                    //Task.Delay(5000).Wait();
                }
            }
            catch
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
