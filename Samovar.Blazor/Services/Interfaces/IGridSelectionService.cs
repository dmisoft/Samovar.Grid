using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public interface IGridSelectionService<T>
    {
        BehaviorSubject<GridSelectionMode> SelectionMode { get; }

        BehaviorSubject<T> SingleSelectedDataRow { get; }
        
        BehaviorSubject<IEnumerable<T>> MultipleSelectedDataRows { get; }
        
        Task OnRowSelected(T dataItem);

        Func<Task> SingleSelectedRowCallback { get; set; }

        Func<Task> MultipleSelectedRowsCallback { get; set; }
    }
}
