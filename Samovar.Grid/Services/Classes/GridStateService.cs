﻿using Microsoft.AspNetCore.Components;
using System.Reactive.Subjects;

namespace Samovar.Grid
{
    public class GridStateService
        : IGridStateService
    {
        public BehaviorSubject<Task<DataSourceState>> DataSourceState { get; } = new BehaviorSubject<Task<DataSourceState>>(Task.FromResult(Grid.DataSourceState.NoData));
        public BehaviorSubject<DataEditState> DataEditState { get; } = new BehaviorSubject<DataEditState>(Grid.DataEditState.Idle);

        public Func<Task>? ShowDataPanelDelegate { get; set; }
        public Func<Task>? CloseDataPanelDelegate { get; set; }

        public Func<Task>? ShowNoDataPanelDelegate { get; set; }
        public Func<Task>? CloseNoDataPanelDelegate { get; set; }

        public Func<Task>? ShowNoDataFoundPanelDelegate { get; set; }
        public Func<Task>? CloseNoDataFoundPanelDelegate { get; set; }

        public Func<Task>? ShowProcessingDataPanelDelegate { get; set; }
        public Func<Task>? CloseProcessingDataPanelDelegate { get; set; }
        public Func<Task>? ShowPagingPanelDelegate { get; set; }
        public Func<Task>? HidePagingPanelDelegate { get; set; }
        public List<EventCallback<DataSourceState>> DataSourceStateEvList { get; set; } = new List<EventCallback<DataSourceState>>();
    }
}
