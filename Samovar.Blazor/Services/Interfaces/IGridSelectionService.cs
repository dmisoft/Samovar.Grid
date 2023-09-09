using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public interface IGridSelectionService<T>
    {
        ISubject<GridSelectionMode> SelectionMode { get; }

        ISubject<T> SingleSelectedDataRow { get; }
        
        ISubject<IEnumerable<T>> MultipleSelectedDataRows { get; }
        
        Task OnRowSelected(T dataItem);

        Func<Task> SingleSelectedRowCallback { get; set; }

        Func<Task> MultipleSelectedRowsCallback { get; set; }
    }
}
