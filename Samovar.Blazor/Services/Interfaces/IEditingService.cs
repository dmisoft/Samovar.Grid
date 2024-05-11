using Microsoft.AspNetCore.Components;
using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public interface IEditingService<T>
    {
        BehaviorSubject<DataGridEditMode> EditMode { get; }

        public BehaviorSubject<EventCallback<T>> OnRowEditBegin { get; }

        public BehaviorSubject<EventCallback> OnRowInsertBegin { get; }

        public BehaviorSubject<EventCallback<T>> OnRowInserting { get; }
        
        public BehaviorSubject<EventCallback<T>> OnRowRemoving { get; }

        Task RowEditCancel();

        Task RowEditBegin(SmDataGridRowModel<T> rowModel);

        Task RowDeleteBegin(SmDataGridRowModel<T> rowModel);

        Task RowEditCommit();

        Task RowInsertBegin();
        Task RowInsertCommit(T dataItem);
        Task RowInsertCancel();

        Func<SmDataGridRowModel<T>, Task>? ShowInsertingPopupDelegate { get; set; }
        Func<Task>? CloseInsertingPopupDelegate { get; set; }

        Func<SmDataGridRowModel<T>, Task>? ShowEditingPopupDelegate { get; set; }
        Func<Task>? CloseEditingPopupDelegate { get; set; }

        Func<SmDataGridRowModel<T>, Task>? ShowInsertingFormDelegate { get; set; }

        Func<Task>? CloseInsertingFormDelegate { get; set; }

        Func<T, Task<string>>? EditingFormTitleDelegate { get; set; }

        event Func<Task> RowEditingEnded;
    }
}
