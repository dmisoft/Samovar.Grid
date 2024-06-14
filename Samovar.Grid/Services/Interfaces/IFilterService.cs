
using Samovar.Grid.Filter;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;

namespace Samovar.Grid
{
    public interface IFilterService
    {
        BehaviorSubject<IEnumerable<GridFilterCellInfo>> FilterInfo { get; }
        ObservableCollection<GridFilterCellInfo> ColumnFilters { get; }
        void AddOrRemoveFilter(GridFilterCellInfo filterCellInfo);
        T? TryGetFilterCellValue<T>(IDataColumnModel columnModel);
        Task ClearFilter();
        event Func<Task> FilterCleared;
    }
}
