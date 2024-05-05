using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public interface IGridStateService
    {
        BehaviorSubject<DataSourceState> DataSourceState { get; }
        //EventCallback<DataSourceStateEnum> DataSourceStateEv { get; set; }
        List<EventCallback<DataSourceState>> DataSourceStateEvList { get; set; }
        BehaviorSubject<DataEditState> DataEditState { get; }

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
