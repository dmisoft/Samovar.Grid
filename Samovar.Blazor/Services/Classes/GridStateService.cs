using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
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
        public EventCallback<DataSourceStateEnum> DataSourceStateEv { get; set; }
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
        public List<EventCallback<DataSourceStateEnum>> DataSourceStateEvList { get; set; } = new List<EventCallback<DataSourceStateEnum>>();

        public GridStateService(IInitService initService, INavigationService navigationService)
        {
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
            //await CloseDataPanelDelegate?.Invoke();
            await CloseNoDataPanelDelegate?.Invoke();
            await CloseNoDataFoundPanelDelegate?.Invoke();
            await CloseProcessingDataPanelDelegate?.Invoke();
            await HidePagingPanelDelegate?.Invoke();

            switch (dataSourceState)
            {
                case DataSourceStateEnum.Idle:
                    //await ShowDataPanelDelegate.Invoke();
                    //if (_navigationService.NavigationMode.Value == DataGridNavigationMode.Paging)
                    //    await ShowPagingPanelDelegate.Invoke();
                    break;
                case DataSourceStateEnum.Loading:
                    //await ShowProcessingDataPanelDelegate.Invoke();
                    break;
                case DataSourceStateEnum.NoData:
                    await ShowNoDataPanelDelegate.Invoke();
                    break;
                default:
                    break;
            }
        }

        public void Dispose()
        {

        }
    }
}
