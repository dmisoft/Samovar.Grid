using System;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public interface IGridStateService
    {
        ISubject<DataSourceStateEnum> DataSourceState { get; }
        ISubject<DataEditStateEnum> DataEditState { get; }

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
