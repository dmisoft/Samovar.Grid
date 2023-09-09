using Samovar.DataGrid;
using Samovar.DataGrid.Data.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Samovar.DataGrid.Data
{
    public interface IJob
    {
    }

    public class RxQueuePubSub
    {
        Subject<IJob> _jobs = new Subject<IJob>();
        private IConnectableObservable<IJob> _connectableObservable;

        public RxQueuePubSub()
        {
            _connectableObservable = _jobs.ObserveOn(Scheduler.Default).Publish();
            _connectableObservable.Connect();
        }

        public void Enqueue(IJob job)
        {
            _jobs.OnNext(job);
        }

        public void RegisterHandler<T>(Action<T> handleAction) where T : IJob
        {
            _connectableObservable.OfType<T>().Subscribe(handleAction);
        }
    }

    internal interface IJob_LoadGridViewModel<T> {
        public IEnumerable<T> Data { get; set; }
        public SamovarGrid<T> Grid { get; set; }
        public GridNavigationService NavigationService { get; set; }
        public GridColumnService ColumnService { get; set; }

    }
    
    internal class Job_LoadGridViewModel<T> 
        : IJob
    {
        public IEnumerable<T> Data { get; set; }
        public SamovarGrid<T> Grid { get; set; }
    }
    internal class Job_LoadInitFilteredViewModel<T>
        : IJob
    {
        public IEnumerable<T> Data { get; set; }
        public SamovarGrid<T> Grid { get; set; }
    }

    internal class Job_CalculateInittialPageInfo<T>
        : IJob
    {
        public int DataCount { get; set; }
        public GridNavigationService NavigationService { get; set; } 
    }

    internal class Job_SetPagerSize<T>
        : IJob
    {
        public string Direction { get; set; }
        public GridNavigationService NavigationService { get; set; }
    }

    internal class Job_NavigateToPage<T>
        : IJob
    {
        public string Direction { get; set; }
        public GridNavigationService NavigationService { get; set; }
    }

    internal class Job_NavigateToSelectedPage<T>
        : IJob
    {
        public int Page { get; set; }
        public GridNavigationService NavigationService { get; set; }
    }

    

    public class Rx<T> {
        public RxQueuePubSub rxQueuePubSub = new RxQueuePubSub();
        internal IGridModelService<T> GridModelService { get; set; } = new GridModelService<T>();

        public Rx()
        {
        }

        public void RegisterHandler() {
            rxQueuePubSub.RegisterHandler<Job_SetPagerSize<T>>(j =>
            {
                j.NavigationService.SetPagerSize(j.Direction);
            });

            rxQueuePubSub.RegisterHandler<Job_NavigateToPage<T>>(j =>
            {
                j.NavigationService.NavigateToPage(j.Direction);
            });

            rxQueuePubSub.RegisterHandler<Job_NavigateToSelectedPage<T>>(j =>
            {
                j.NavigationService.CurrentPage = j.Page;
            });

            rxQueuePubSub.RegisterHandler<Job_CalculateInittialPageInfo<T>>(j =>
            {
                j.NavigationService.CalculateInitPaginationInfo(j.DataCount);
            });

            rxQueuePubSub.RegisterHandler<Job_LoadGridViewModel<T>>(j =>
            {
                j.Grid.GuiStateService.Loading = true;
                GridModelService.Init(j.Data, j.Grid.GridColumnService.Columns);

                GridModelService.InitRepository(j.Data);

                GridModelService.LoadViewCollection_V6(j.Grid.NavigationService.CurrentPage, j.Grid.NavigationService.PagerSize, GridModelService.FilterMode, j.Grid.GridColumnService.SortingColumn, j.Grid.GridColumnService.SortingAscending);

                //j.NavigationService.CalculateInitPaginationInfo(GridModelService.TotalDataItemsCount);
                j.Grid.GuiStateService.Loading = false;

                j.Grid.Refresh();
            });

            rxQueuePubSub.RegisterHandler<Job_LoadInitFilteredViewModel<T>>(j =>
            {
                j.Grid.GuiStateService.Filtering = true;

                GridModelService.Init(j.Data, j.Grid.GridColumnService.Columns);

                GridModelService.InitRepository(j.Data);

                GridModelService.LoadViewCollection_V6(1, j.Grid.NavigationService.PagerSize, GridModelService.FilterMode, j.Grid.GridColumnService.SortingColumn, j.Grid.GridColumnService.SortingAscending);

                j.Grid.NavigationService.CalculateInitPaginationInfo(GridModelService.TotalDataItemsCount);

                j.Grid.GuiStateService.Filtering = false;

                j.Grid.Refresh();
            });
        }
    }
}
