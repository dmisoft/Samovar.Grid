using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public class GridStateService
        : IGridStateService, IDisposable
    {
        private readonly IInitService _initService;
        private readonly INavigationService _navigationService;

        public BehaviorSubject<DataSourceStateEnum> DataSourceState { get; } = new BehaviorSubject<DataSourceStateEnum>(DataSourceStateEnum.NoData);
        public BehaviorSubject<DataEditStateEnum> DataEditState { get; } = new BehaviorSubject<DataEditStateEnum>(DataEditStateEnum.Idle);

        public Func<Task> ShowDataPanelDelegate { get; set; }
        public Func<Task> CloseDataPanelDelegate { get; set; }

        public Func<Task> ShowNoDataPanelDelegate { get; set; }
        public Func<Task> CloseNoDataPanelDelegate { get; set; }

        public Func<Task> ShowNoDataFoundPanelDelegate { get; set; }
        public Func<Task> CloseNoDataFoundPanelDelegate { get; set; }

        public Func<Task> ShowProcessingDataPanelDelegate { get; set; }
        public Func<Task> CloseProcessingDataPanelDelegate { get; set; }
        public Func<Task> ShowPagingPanelDelegate { get; set; }
        public Func<Task> HidePagingPanelDelegate { get; set; }

        public GridStateService(IInitService initService, INavigationService navigationService)
        {
            //TODO refactoring
            //var querySubscription = new Subscription1TaskVoid<DataSourceStateEnum>(DataSourceState, ProcessDataSourceState).CreateMap();
            //var querySubscription1 = new Subscription1TaskVoid<DataEditStateEnum>(DataEditState, ProcessDataEditState).CreateMap();
            _initService = initService;
            _navigationService = navigationService;

            _initService.IsInitialized.Subscribe(DataGridInitializerCallback);
        }

        private void DataGridInitializerCallback(bool obj)
        {
            DataSourceState.Subscribe(async dataSourceState => await ProcessDataSourceState(dataSourceState));
            DataEditState.Subscribe(ProcessDataEditState);
        }

        private void ProcessDataEditState(DataEditStateEnum arg)
        {
            //return Task.CompletedTask;
        }

        private async Task ProcessDataSourceState(DataSourceStateEnum dataSourceState)
        {
            return;
            await CloseDataPanelDelegate?.Invoke();
            await CloseNoDataPanelDelegate?.Invoke();
            await CloseNoDataFoundPanelDelegate?.Invoke();
            await CloseProcessingDataPanelDelegate?.Invoke();
            await HidePagingPanelDelegate?.Invoke();

            Func<Task> t = null;
            switch (dataSourceState)
            {
                case DataSourceStateEnum.Idle:
                    //t += ShowDataPanelDelegate;
                    //await ShowDataPanelDelegate.Invoke();
                    //t += ShowDataPanelDelegate;
                    //if (_navigationService.NavigationMode.Value == DataGridNavigationMode.Paging)
                    //    await ShowPagingPanelDelegate.Invoke();
                        //t += ShowPagingPanelDelegate;
                    break;
                case DataSourceStateEnum.Loading:
                    //t += ShowProcessingDataPanelDelegate;
                    //await ShowProcessingDataPanelDelegate.Invoke();
                    //return;
                    break;
                case DataSourceStateEnum.NoData:
                    //t += ShowNoDataPanelDelegate;
                    //await ShowNoDataPanelDelegate.Invoke();
                    break;
                default:
                    break;
            }

            //await t?.Invoke();
            
            //if (t != null) {
            //    Delegate[] delList = t.GetInvocationList();

            //    if (delList != null)
            //    {
            //        foreach (Delegate del in delList)
            //        {
            //            t -= (Func<Task>)del;
            //        }
            //    }
            //}
        }

        public void Dispose()
        {

        }
    }
}
