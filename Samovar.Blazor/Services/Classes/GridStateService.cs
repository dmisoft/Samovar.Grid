using System;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public class GridStateService
        : IGridStateService, IDisposable
    {
        private readonly INavigationService _navigationService;

        public ISubject<DataSourceStateEnum> DataSourceState { get; } = new ParameterSubject<DataSourceStateEnum>(DataSourceStateEnum.NoData);
        public ISubject<DataEditStateEnum> DataEditState { get; } = new ParameterSubject<DataEditStateEnum>(DataEditStateEnum.Idle);

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

        public GridStateService(INavigationService navigationService)
        {
            var querySubscription = new Subscription1TaskVoid<DataSourceStateEnum>(DataSourceState, ProcessDataSourceState).CreateMap();
            var querySubscription1 = new Subscription1TaskVoid<DataEditStateEnum>(DataEditState, ProcessDataEditState).CreateMap();
            _navigationService = navigationService;
        }

        private Task ProcessDataEditState(DataEditStateEnum arg)
        {
            //throw new NotImplementedException();
            return Task.CompletedTask;
        }

        private async Task ProcessDataSourceState(DataSourceStateEnum arg)
        {
            CloseDataPanelDelegate?.Invoke();
            CloseNoDataPanelDelegate?.Invoke();
            CloseNoDataFoundPanelDelegate?.Invoke();
            CloseProcessingDataPanelDelegate?.Invoke();
            HidePagingPanelDelegate?.Invoke();

            Func<Task> t = null;
            switch (arg)
            {
                case DataSourceStateEnum.Idle:
                    t += ShowDataPanelDelegate;
                    if(_navigationService.NavigationMode.SubjectValue == DataGridNavigationMode.Paging)
                        t += ShowPagingPanelDelegate;
                    break;
                case DataSourceStateEnum.Loading:
                    t += ShowProcessingDataPanelDelegate;
                    //await ShowProcessingDataPanelDelegate.Invoke();
                    //return;
                    break;
                case DataSourceStateEnum.NoData:
                    t += ShowNoDataPanelDelegate;
                    break;
                default:
                    break;
            }
            
            await t?.Invoke();
            
            Delegate[] delList = t.GetInvocationList();

            if (delList != null)
            {
                foreach (Delegate del in delList)
                {
                    t -= (Func<Task>)del;
                }
            }

            //return Task.CompletedTask;
        }

        public void Dispose()
        {
            
        }
    }
}
