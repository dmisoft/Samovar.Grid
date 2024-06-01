using Samovar.Blazor.Filter;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public class FilterService
        : IFilterService, IAsyncDisposable
    {
        public ObservableCollection<GridFilterCellInfo> ColumnFilters { get; } = new ObservableCollection<GridFilterCellInfo>();

        public BehaviorSubject<IEnumerable<GridFilterCellInfo>> FilterInfo { get; } = new BehaviorSubject<IEnumerable<GridFilterCellInfo>>(new List<GridFilterCellInfo>());

        public FilterService()
        {
            ColumnFilters.CollectionChanged += ColumnFilters_CollectionChanged;
        }

        public void Filter(GridFilterCellInfo filterCellInfo)
        {
            if (!ColumnFilters.Contains(filterCellInfo))
            {
                ColumnFilters.Add(filterCellInfo);
            }
            else
            {
                if (filterCellInfo.FilterCellValue is null)
                    ColumnFilters.RemoveAt(ColumnFilters.IndexOf(filterCellInfo));
                else
                    ColumnFilters[ColumnFilters.IndexOf(filterCellInfo)] = filterCellInfo;
            }
        }

        private void ColumnFilters_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            FilterInfo.OnNext(ColumnFilters.ToList());
        }


        public async Task ClearFilter()
        {
            ColumnFilters.Clear();
            FilterInfo.OnNext(ColumnFilters.ToList());
            await OnFilterCleared();
        }

        public event Func<Task>? FilterCleared;

        public async Task OnFilterCleared()
        {
            if (FilterCleared is not null)
                await FilterCleared.Invoke();
        }

        public T? TryGetFilterCellValue<T>(IDataColumnModel columnModel)
        {
            if (!ColumnFilters.Any(f => f.ColumnModel is not null && f.ColumnModel.Equals(columnModel)))
            {
                return default;
            }
            return (T?)ColumnFilters.Single(f => f.ColumnModel is not null && f.ColumnModel.Equals(columnModel)).FilterCellValue;
        }

        public ValueTask DisposeAsync()
        {
            ColumnFilters.CollectionChanged -= ColumnFilters_CollectionChanged;
            return ValueTask.CompletedTask;
        }
    }
}
