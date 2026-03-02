using System.Reactive.Subjects;

namespace Samovar.Grid;

public interface IGridSelectionService<T>
{
    BehaviorSubject<RowSelectionMode> SelectionMode { get; }

    BehaviorSubject<T?> SingleSelectedDataRow { get; }

    BehaviorSubject<IEnumerable<T>?> MultipleSelectedDataRows { get; }

    Task OnRowSelected(T dataItem);

    Task OnRowChecked(T dataItem);

    Task Reset();

    Task SelectCurrentPage();

    Task SelectAll(IEnumerable<T> allItems);

    Func<Task>? SingleSelectedRowCallback { get; set; }

    Func<Task>? MultipleSelectedRowsCallback { get; set; }
}
