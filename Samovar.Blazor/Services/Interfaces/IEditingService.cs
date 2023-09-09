using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public interface IEditingService<T>
    {
        ISubject<GridEditMode> EditMode { get; }

        public ISubject<EventCallback<T>> OnRowEditBegin { get; }

        public ISubject<EventCallback> OnRowInsertBegin { get; }

        public ISubject<EventCallback<T>> OnRowInserting { get; }
        
        public ISubject<EventCallback<T>> OnRowRemoving { get; }

        Task RowEditCancel();

        Task RowEditBegin(SmDataGridRowModel<T> rowModel);

        Task RowDeleteBegin(SmDataGridRowModel<T> rowModel);

        Task RowEditCommit();

        Task RowInsertBegin();
        Task RowInsertCommit(T dataItem);
        Task RowInsertCancel();

        Func<SmDataGridRowModel<T>, Task> ShowInsertingPopupDelegate { get; set; }
        Func<Task> CloseInsertingPopupDelegate { get; set; }

        Func<SmDataGridRowModel<T>, Task> ShowEditingPopupDelegate { get; set; }
        Func<Task> CloseEditingPopupDelegate { get; set; }

        Func<SmDataGridRowModel<T>, Task> ShowInsertingFormDelegate { get; set; }

        Func<Task> CloseInsertingFormDelegate { get; set; }

        Func<T, Task<string>> GetEditingFormTitleDelegate { get; set; }

        event Func<Task> RowEditingEnded;
    }
}
