using Microsoft.AspNetCore.Components;
using System.Reactive.Subjects;

namespace Samovar.Grid
{
    public interface IEditingService<T>
    {
        BehaviorSubject<GridEditMode> EditMode { get; }

        public EventCallback<T> OnRowEditBegin { get; set; }

        public BehaviorSubject<EventCallback> OnRowInsertBegin { get; }

        public BehaviorSubject<EventCallback<T>> OnRowInserting { get; }

        public BehaviorSubject<EventCallback<T>> OnRowRemoving { get; }

        Task RowEditCancel();

        Task RowEditBegin(GridRowModel<T> rowModel);

        Task RowDeleteBegin(GridRowModel<T> rowModel);

        Task RowEditCommit();

        Task RowInsertBegin();
        Task RowInsertCommit(T dataItem);
        Task RowInsertCancel();

        Func<GridRowModel<T>, Task>? ShowInsertingPopupDelegate { get; set; }
        Func<Task>? CloseInsertingPopupDelegate { get; set; }

        Func<GridRowModel<T>, Task>? ShowEditingPopupDelegate { get; set; }
        Func<Task>? CloseEditingPopupDelegate { get; set; }

        Func<GridRowModel<T>, Task>? ShowInsertingFormDelegate { get; set; }

        Func<Task>? CloseInsertingFormDelegate { get; set; }

        Func<T, Task<string>>? EditingFormTitleDelegate { get; set; }

        event Func<Task> RowEditingEnded;
    }
}
