using Samovar.Blazor.Filter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public class FilterService
        : IFilterService, IDisposable
    {
        public ObservableCollection<DataGridFilterCellInfo> ColumnFilters { get; } = new ObservableCollection<DataGridFilterCellInfo>();

        public BehaviorSubject<IEnumerable<DataGridFilterCellInfo>> FilterInfo { get; } = new BehaviorSubject<IEnumerable<DataGridFilterCellInfo>>(new List<DataGridFilterCellInfo>());

        public FilterService()
        {
            ColumnFilters.CollectionChanged += ColumnFilters_CollectionChanged;
        }

        public void Filter(DataGridFilterCellInfo filterCellInfo)
        {
            if (!ColumnFilters.Contains(filterCellInfo))
            {
                ColumnFilters.Add(filterCellInfo);
            }
            else {
                if (filterCellInfo.FilterCellValue is null)
                    ColumnFilters.RemoveAt(ColumnFilters.IndexOf(filterCellInfo));
                else
                    ColumnFilters[ColumnFilters.IndexOf(filterCellInfo)] = filterCellInfo;
            }
        }

        private void ColumnFilters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }

            FilterInfo.OnNext(ColumnFilters.ToList());
        }

        public void Dispose()
        {
            ColumnFilters.CollectionChanged -= ColumnFilters_CollectionChanged;
        }

        public async Task ClearFilter()
        {
            ColumnFilters.Clear();
            FilterInfo.OnNext(ColumnFilters.ToList());
            await OnFilterCleared();
        }
        
        public event Func<Task> FilterCleared;

        public async Task OnFilterCleared()
        {
            if (FilterCleared != null)
            {
                await FilterCleared.Invoke();
            }
        }

		public T TryGetFilterCellValue<T>(IDataColumnModel columnModel)
		{
            if (!ColumnFilters.Any(f => f.ColumnMetadata.Equals(columnModel)))
            {
                return default;
            }

            return (T)ColumnFilters.Single(f => f.ColumnMetadata.Equals(columnModel)).FilterCellValue;
        }
    }
}
