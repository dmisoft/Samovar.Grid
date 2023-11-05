using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public class EditingService<T>
        : IEditingService<T>, IDisposable
    {
        SmDataGridRowModel<T> _editingRowModel;

        IGridStateService _stateService;
        IRepositoryService<T> _repositoryService;
        IColumnService _columnService;
        INavigationService _navigationService;

        public event Func<Task> RowEditingEnded;
        
        public async Task OnRowEditingEnded()
        {
            if (RowEditingEnded != null)
            {
                await RowEditingEnded.Invoke();
            }
        }

        public BehaviorSubject<GridEditMode> EditMode { get; } = new BehaviorSubject<GridEditMode>(GridEditMode.Form);

        public BehaviorSubject<EventCallback<T>> OnRowEditBegin { get; } = new BehaviorSubject<EventCallback<T>>(default(EventCallback<T>));
        public BehaviorSubject<EventCallback<Dictionary<string, object>>> OnInitializeNewRow { get; } = new BehaviorSubject<EventCallback<Dictionary<string, object>>>(default(EventCallback<Dictionary<string, object>>));
        public BehaviorSubject<EventCallback> OnRowInsertBegin { get; } = new BehaviorSubject<EventCallback>(default(EventCallback));
        public BehaviorSubject<EventCallback<T>> OnRowInserting { get; } = new BehaviorSubject<EventCallback<T>>(default(EventCallback<T>));
        public BehaviorSubject<EventCallback<T>> OnRowRemoving { get; } = new BehaviorSubject<EventCallback<T>>(default(EventCallback<T>));
        public Func<SmDataGridRowModel<T>, Task> ShowInsertingPopupDelegate { get; set; }
        public Func<Task> CloseInsertingPopupDelegate { get; set; }
        public Func<SmDataGridRowModel<T>, Task> ShowEditingPopupDelegate { get; set; }
        public Func<Task> CloseEditingPopupDelegate { get; set; }

        public Func<SmDataGridRowModel<T>, Task> ShowInsertingFormDelegate { get; set; }
        public Func<Task> CloseInsertingFormDelegate { get; set; }
		public Func<T, Task<string>> EditingFormTitleDelegate { get; set; }

		public EditingService(
              IGridStateService stateService
            , IRepositoryService<T> repositoryService
            , IColumnService columnService
            , INavigationService navigationService)
        {
            _stateService = stateService;
            _repositoryService = repositoryService;
            _columnService = columnService;
            _navigationService = navigationService;
        }

        public async Task RowEditBegin(SmDataGridRowModel<T> rowModel)
        {
            await OnRowEditBegin.Value.InvokeAsync(rowModel.DataItem);

            _editingRowModel = rowModel;
            _editingRowModel.RowState = SmDataGridRowState.Editing;

            _editingRowModel.CreateEditingModel();

            if (_navigationService.NavigationMode.Value == DataGridNavigationMode.VirtualScrolling) {
                ShowEditingPopupDelegate?.Invoke(_editingRowModel);
            }
            else {
                if (EditMode.Value == GridEditMode.Popup)
                    ShowEditingPopupDelegate?.Invoke(_editingRowModel);
            }

            _stateService.DataEditState.OnNext(DataEditStateEnum.Editing);
        }

        public Task RowEditCancel()
        {
            _editingRowModel.RowState = SmDataGridRowState.Idle;

            _editingRowModel = null;

            if (_navigationService.NavigationMode.Value == DataGridNavigationMode.VirtualScrolling)
            {
                CloseEditingPopupDelegate?.Invoke();
            }
            else
            {
                if (EditMode.Value == GridEditMode.Popup)
                    CloseEditingPopupDelegate?.Invoke();
            }

            return Task.CompletedTask;
        }

        public async Task RowEditCommit()
        {
            _editingRowModel.RowState = SmDataGridRowState.Idle;
            _editingRowModel.CommitEditingModel();
            _editingRowModel = null;

            if (EditMode.Value == GridEditMode.Popup)
                CloseEditingPopupDelegate?.Invoke();

            await OnRowEditingEnded();
        }

        public async Task RowDeleteBegin(SmDataGridRowModel<T> rowModel)
        {
            await OnRowRemoving.Value.InvokeAsync(rowModel.DataItem);
        }

        public async Task RowInsertBegin()
        {
            var rowModel = new SmDataGridRowModel<T>((T)Activator.CreateInstance(typeof(T)), _columnService.DataColumnModels, 0, _repositoryService.PropInfo, false);
            rowModel.CreateEditingModel();

            await OnRowInsertBegin.Value.InvokeAsync();

            if (EditMode.Value == GridEditMode.Popup)
                ShowInsertingPopupDelegate?.Invoke(rowModel);
            else if (EditMode.Value == GridEditMode.Form)
                ShowInsertingFormDelegate?.Invoke(rowModel);

            _stateService.DataEditState.OnNext(DataEditStateEnum.Inserting);
        }

        public async Task RowInsertCommit(T dataItem)
        {
            await OnRowInserting.Value.InvokeAsync(dataItem);

            if (EditMode.Value == GridEditMode.Popup)
                CloseInsertingPopupDelegate?.Invoke();
            else if (EditMode.Value == GridEditMode.Form)
                CloseInsertingFormDelegate?.Invoke();
        }

        public Task RowInsertCancel()
        {
            if (EditMode.Value == GridEditMode.Popup)
                CloseInsertingPopupDelegate?.Invoke();
            else if (EditMode.Value == GridEditMode.Form)
                CloseInsertingFormDelegate?.Invoke();

            return Task.CompletedTask;
        }

        public void Dispose()
        {

        }
	}
}
