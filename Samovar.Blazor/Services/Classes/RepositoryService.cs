using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public class RepositoryService<T>
        : IRepositoryService<T>
    {
        public IEnumerable<SmDataGridRowModel<T>> ViewCollection { get; private set; }

        private readonly IDataSourceService<T> _dataSourceService;
        INavigationService _navigationService;
        IColumnService _columnService;
        IInitService _initService;
        IRowDetailService<T> _rowDetailService;
        private bool repositoryForVirtualScrollingInitialized;
        private readonly IGridStateService _stateService;
        private readonly ILayoutService _layoutService;

        public ISubject<HashSet<T>> Data { get; private set; } = new ParameterSubject<HashSet<T>>(new HashSet<T>());
        
        public Dictionary<string, PropertyInfo> PropInfo { get; } = new Dictionary<string, PropertyInfo>();
        //public static Dictionary<string, PropertyInfo> PropInfoStatic { get; } = new Dictionary<string, PropertyInfo>();
        public static Dictionary<string, Func<T, int>> PropInfoDelegateInt { get; } = new Dictionary<string, Func<T, int>>();
        public static Dictionary<string, Func<T, string>> PropInfoDelegateString{ get; } = new Dictionary<string, Func<T, string>>();
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

            //Func<object, string>> emit = Func<object, string>.DynamicMethod
            //DynamicMethod
            //Expression.Property
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
                        if(!PropInfoDelegateInt.ContainsKey(pi.Name))
                            PropInfoDelegateInt.Add(pi.Name, (Func<T, int>)Delegate.CreateDelegate(typeof(Func<T, int>), pi.GetGetMethod(true)!));
                        break;
                    default:
                        break;
                }

                //PropInfoDelegate.Add(pi.Name, (Func<T, object>)Delegate.CreateDelegate(typeof(Func<T, object>), pi.GetGetMethod(true)!));
                //PropInfoDelegate.Add(pi.Name, (Func<T, int>)Delegate.CreateDelegate(typeof(Func<T, int>), pi.CreateGetPropertyDelegate()));//CreateGetPropertyDelegate
            }

            _initService.IsInitialized.Subscribe(DataGridInitializerCallback);

            var querySubscription = new Subscription2TaskVoid<IQueryable<T>, NavigationStrategyDataLoadingSettings>(_dataSourceService.DataQuery, _dataSourceService.DataLoadingSettings, DataLoadingSettingsCallback2).CreateMap();
        }

        private async Task DataLoadingSettingsCallback2(IQueryable<T> query, NavigationStrategyDataLoadingSettings loadingSettings)
        {
            if (query == null)
            {
                _stateService.DataSourceState.OnNextParameterValue(DataSourceStateEnum.NoData);
                return;
            }

            query = query.Skip(loadingSettings.Skip).Take(loadingSettings.Take);

            if (_navigationService.NavigationMode.SubjectValue == DataGridNavigationMode.Paging)
            {
                _stateService.DataSourceState.OnNextParameterValue(DataSourceStateEnum.Loading);
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                ViewCollection = await CreateRowModelList(query, _columnService.DataColumnModels, PropInfo);
                stopWatch.Stop();

                if (ViewCollection.Count() == 0)
                {
                    _stateService.DataSourceState.OnNextParameterValue(DataSourceStateEnum.NoData);
                }
                else
                {
                    _stateService.DataSourceState.OnNextParameterValue(DataSourceStateEnum.Idle);
                }
            }
            else if (_navigationService.NavigationMode.SubjectValue == DataGridNavigationMode.VirtualScrolling && !repositoryForVirtualScrollingInitialized)
            {
                _stateService.DataSourceState.OnNextParameterValue(DataSourceStateEnum.Idle);
                repositoryForVirtualScrollingInitialized = true;
            }
            else if (_navigationService.NavigationMode.SubjectValue == DataGridNavigationMode.VirtualScrolling && repositoryForVirtualScrollingInitialized)
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                ViewCollection = await CreateRowModelList(query, _columnService.DataColumnModels, PropInfo);

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
                    _stateService.DataSourceState.OnNextParameterValue(DataSourceStateEnum.NoData);
                }
                else
                {
                    //TODO extra Idle state for virtual scrolling???
                    _stateService.DataSourceState.OnNextParameterValue(DataSourceStateEnum.Idle);
                }


            }

            await OnViewCollectionChanged(ViewCollection);
        }

        public event Func<IEnumerable<SmDataGridRowModel<T>>, Task> ViewCollectionChanged;
        internal async Task OnViewCollectionChanged(IEnumerable<SmDataGridRowModel<T>> data)
        {
            if (ViewCollectionChanged != null)
            {
                await ViewCollectionChanged.Invoke(data);
            }
        }

        public Subscription1<IEnumerable<T>, int> DataChangeSubscription { get; }

        private void DataGridInitializerCallback(bool obj)
        {
        }

        private async Task<List<SmDataGridRowModel<T>>> CreateRowModelList(IQueryable<T> gridData, IEnumerable<IDataColumnModel> ColumnMetadataList, Dictionary<string, PropertyInfo> PropInfo)
        {
            var retVal = new List<SmDataGridRowModel<T>>();
            int rowPosition = 0;

            try
            {
                foreach (var keyDataPair in gridData.ToHashSet())
                {
                    rowPosition++;
                    retVal.Add(new SmDataGridRowModel<T>(keyDataPair, ColumnMetadataList, rowPosition, PropInfo, _rowDetailService.ExpandedRowDetails.SubjectValue.Any(r => r.Equals(keyDataPair))));
                    //await Task.Delay(5);
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
