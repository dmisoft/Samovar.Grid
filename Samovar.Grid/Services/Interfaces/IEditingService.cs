using Microsoft.AspNetCore.Components;
using System.Reactive.Subjects;

namespace Samovar.Grid;

public interface IEditingService<T>
{
    BehaviorSubject<GridEditMode> EditMode { get; }
    EventCallback<T> OnRowEditBegin { get; set; }
    EventCallback OnRowInsertBegin { get; set; }
    EventCallback<T> OnRowInserting { get; set; }
    EventCallback<T> OnRowRemoving { get; set; }
    Task CancelRowEdit(GridRowModel<T> rowModel);
    Task CommitCustomRowEdit(T item);
    Task EditBegin(GridRowModel<T> rowModel);
    Task RowDeleteBegin(GridRowModel<T> rowModel);
    Task EditCommit(GridRowModel<T> rowModel);
    Task RowInsertBegin();
    Task RowInsertCommit(T dataItem);
    Task RowInsertCancel();
    Task CancelCustomRowEdit(T item);
    Func<GridRowModel<T>, Task>? ShowInsertingPopupDelegate { get; set; }
    Func<Task>? CloseInsertingPopupDelegate { get; set; }
    Func<GridRowModel<T>, Task>? ShowEditingPopupDelegate { get; set; }
    Func<Task>? CloseEditingPopupDelegate { get; set; }
    Func<GridRowModel<T>, Task>? ShowInsertingFormDelegate { get; set; }
    Func<Task>? CloseInsertingFormDelegate { get; set; }
    event Func<Task> RowEditingEnded;
}
