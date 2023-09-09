using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public ISubject<GridEditMode> EditMode { get; } = new ParameterSubject<GridEditMode>(GridEditMode.Form);

        public ISubject<EventCallback<T>> OnRowEditBegin { get; } = new ParameterSubject<EventCallback<T>>(default(EventCallback<T>));
        public ISubject<EventCallback<Dictionary<string, object>>> OnInitializeNewRow { get; } = new ParameterSubject<EventCallback<Dictionary<string, object>>>(default(EventCallback<Dictionary<string, object>>));
        public ISubject<EventCallback> OnRowInsertBegin { get; } = new ParameterSubject<EventCallback>(default(EventCallback));
        public ISubject<EventCallback<T>> OnRowInserting { get; } = new ParameterSubject<EventCallback<T>>(default(EventCallback<T>));
        public ISubject<EventCallback<T>> OnRowRemoving { get; } = new ParameterSubject<EventCallback<T>>(default(EventCallback<T>));
        public Func<SmDataGridRowModel<T>, Task> ShowInsertingPopupDelegate { get; set; }
        public Func<Task> CloseInsertingPopupDelegate { get; set; }
        public Func<SmDataGridRowModel<T>, Task> ShowEditingPopupDelegate { get; set; }
        public Func<Task> CloseEditingPopupDelegate { get; set; }

        public Func<SmDataGridRowModel<T>, Task> ShowInsertingFormDelegate { get; set; }
        public Func<Task> CloseInsertingFormDelegate { get; set; }
		public Func<T, Task<string>> GetEditingFormTitleDelegate { get; set; }

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
            _repositoryService.DetachViewCollectionSubscription();

            await OnRowEditBegin.SubjectValue.InvokeAsync(rowModel.DataItem);

            _editingRowModel = rowModel;
            _editingRowModel.RowState = SmDataGridRowState.Editing;

            _editingRowModel.CreateEditingModel();

            if (_navigationService.NavigationMode.SubjectValue == DataGridNavigationMode.VirtualScrolling) {
                ShowEditingPopupDelegate?.Invoke(_editingRowModel);
            }
            else {
                if (EditMode.SubjectValue == GridEditMode.Popup)
                    ShowEditingPopupDelegate?.Invoke(_editingRowModel);
            }

            _stateService.DataEditState.OnNextParameterValue(DataEditStateEnum.Editing);
        }

        public Task RowEditCancel()
        {
            _editingRowModel.RowState = SmDataGridRowState.Idle;

            _editingRowModel = null;

            _repositoryService.AttachViewCollectionSubscription();

            if (_navigationService.NavigationMode.SubjectValue == DataGridNavigationMode.VirtualScrolling)
            {
                CloseEditingPopupDelegate?.Invoke();
            }
            else
            {
                if (EditMode.SubjectValue == GridEditMode.Popup)
                    CloseEditingPopupDelegate?.Invoke();
            }

            return Task.CompletedTask;
        }

        public async Task RowEditCommit()
        {
            _editingRowModel.RowState = SmDataGridRowState.Idle;

            _editingRowModel.CommitEditingModel();

            _editingRowModel = null;

            _repositoryService.AttachViewCollectionSubscription();

            if (EditMode.SubjectValue == GridEditMode.Popup)
                CloseEditingPopupDelegate?.Invoke();

            await OnRowEditingEnded();
        }

        public async Task RowDeleteBegin(SmDataGridRowModel<T> rowModel)
        {
            await OnRowRemoving.SubjectValue.InvokeAsync(rowModel.DataItem);
        }

        public async Task RowInsertBegin()
        {
            var rowModel = new SmDataGridRowModel<T>((T)Activator.CreateInstance(typeof(T)), _columnService.DataColumnModels, 0, _repositoryService.PropInfo, false);
            rowModel.CreateEditingModel();

            await OnRowInsertBegin.SubjectValue.InvokeAsync();

            if (EditMode.SubjectValue == GridEditMode.Popup)
                ShowInsertingPopupDelegate?.Invoke(rowModel);
            else if (EditMode.SubjectValue == GridEditMode.Form)
                ShowInsertingFormDelegate?.Invoke(rowModel);

            _stateService.DataEditState.OnNextParameterValue(DataEditStateEnum.Inserting);
        }

        public async Task RowInsertCommit(T dataItem)
        {
            await OnRowInserting.SubjectValue.InvokeAsync(dataItem);

            if (EditMode.SubjectValue == GridEditMode.Popup)
                CloseInsertingPopupDelegate?.Invoke();
            else if (EditMode.SubjectValue == GridEditMode.Form)
                CloseInsertingFormDelegate?.Invoke();
        }

        public Task RowInsertCancel()
        {
            if (EditMode.SubjectValue == GridEditMode.Popup)
                CloseInsertingPopupDelegate?.Invoke();
            else if (EditMode.SubjectValue == GridEditMode.Form)
                CloseInsertingFormDelegate?.Invoke();

            return Task.CompletedTask;
        }

        public void Dispose()
        {

        }
	}
}
