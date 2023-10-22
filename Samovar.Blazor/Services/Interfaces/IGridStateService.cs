using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public interface IGridStateService
    {
        BehaviorSubject<DataSourceStateEnum> DataSourceState { get; }
        BehaviorSubject<DataEditStateEnum> DataEditState { get; }

        Func<Task> ShowDataPanelDelegate { get; set; }
        Func<Task> CloseDataPanelDelegate { get; set; }

        Func<Task> ShowNoDataPanelDelegate { get; set; }
        Func<Task> CloseNoDataPanelDelegate { get; set; }
        
        Func<Task> ShowNoDataFoundPanelDelegate { get; set; }
        Func<Task> CloseNoDataFoundPanelDelegate { get; set; }
        
        Func<Task> ShowProcessingDataPanelDelegate { get; set; }
        Func<Task> CloseProcessingDataPanelDelegate { get; set; }

        Func<Task> ShowPagingPanelDelegate { get; set; }
        Func<Task> HidePagingPanelDelegate { get; set; }

    }
}
