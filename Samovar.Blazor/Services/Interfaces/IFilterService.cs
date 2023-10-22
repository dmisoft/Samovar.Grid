
using Samovar.Blazor.Filter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public interface IFilterService
    {
        BehaviorSubject<IEnumerable<DataGridFilterCellInfo>> FilterInfo { get; }
        ObservableCollection<DataGridFilterCellInfo> ColumnFilters { get; }
        void Filter(DataGridFilterCellInfo filterCellInfo);
        T TryGetFilterCellValue<T>(IDataColumnModel columnModel);
        Task ClearFilter();
        event Func<Task> FilterCleared;
	}
}
