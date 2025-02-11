﻿using Microsoft.AspNetCore.Components;
using System.Reactive.Subjects;

namespace Samovar.Grid;

public class EditingService<T>(
          IGridStateService _stateService
        , IRepositoryService<T> _repositoryService
        , IColumnService _columnService
        , INavigationService _navigationService)
    : IEditingService<T>, IAsyncDisposable
{
    private GridRowModel<T>? _editingRowModel;

    public event Func<Task>? RowEditingEnded;

    public async Task OnRowEditingEnded()
    {
        if (RowEditingEnded != null)
        {
            await RowEditingEnded.Invoke();
        }
    }

    public BehaviorSubject<GridEditMode> EditMode { get; } = new BehaviorSubject<GridEditMode>(GridEditMode.Form);
    public EventCallback<T> OnRowEditBegin { get; set; }
    public EventCallback OnRowInsertBegin { get; set; }
    public EventCallback<T> OnRowInserting { get; set; }
    public EventCallback<T> OnRowRemoving { get; set; }

    public Func<GridRowModel<T>, Task>? ShowInsertingPopupDelegate { get; set; }
    public Func<Task>? CloseInsertingPopupDelegate { get; set; }
    public Func<GridRowModel<T>, Task>? ShowEditingPopupDelegate { get; set; }
    public Func<Task>? CloseEditingPopupDelegate { get; set; }

    public Func<GridRowModel<T>, Task>? ShowInsertingFormDelegate { get; set; }
    public Func<Task>? CloseInsertingFormDelegate { get; set; }

    public async Task EditBegin(GridRowModel<T> rowModel)
    {
        if(_editingRowModel is not null)
            await CancelRowEdit(_editingRowModel);
        
        _editingRowModel = rowModel;

        await OnRowEditBegin.InvokeAsync(rowModel.DataItem);

        rowModel.RowState.OnNext(GridRowState.Editing);
        rowModel.CreateEditingModel();

        if (_navigationService.NavigationMode.Value == NavigationMode.Virtual)
        {
            ShowEditingPopupDelegate?.Invoke(rowModel);
        }
        else
        {
            if (EditMode.Value == GridEditMode.Popup)
                ShowEditingPopupDelegate?.Invoke(rowModel);
        }

        _stateService.DataEditState.OnNext(DataEditState.Editing);
    }

    public Task CancelRowEdit(GridRowModel<T> rowModel)
    {
        rowModel.RowState.OnNext(GridRowState.Idle);
        _editingRowModel = null;

        if (EditMode.Value == GridEditMode.Popup || _navigationService.NavigationMode.Value == NavigationMode.Virtual)
            CloseEditingPopupDelegate?.Invoke();

        return Task.CompletedTask;
    }


    public async Task EditCommit(GridRowModel<T> rowModel)
    {
        rowModel.RowState.OnNext(GridRowState.Idle);
        rowModel.EditCommit();
        _editingRowModel = null;


        if (EditMode.Value == GridEditMode.Popup || _navigationService.NavigationMode.Value == NavigationMode.Virtual)
            CloseEditingPopupDelegate?.Invoke();

        await OnRowEditingEnded();
    }

    public async Task CommitCustomRowEdit(T item)
    {
        if (_editingRowModel is not null)
        {
            _editingRowModel.RowState.OnNext(GridRowState.Idle);
            _editingRowModel.CommitCustomEdit();
            _editingRowModel = null;


            if (EditMode.Value == GridEditMode.Popup || _navigationService.NavigationMode.Value == NavigationMode.Virtual)
                CloseEditingPopupDelegate?.Invoke();
        }

        await OnRowEditingEnded();
    }

    public Task CancelCustomRowEdit(T item)
    {
        if (_editingRowModel is not null)
        {
            _editingRowModel.RowState.OnNext(GridRowState.Idle);
            _editingRowModel = null;

            if (EditMode.Value == GridEditMode.Popup || _navigationService.NavigationMode.Value == NavigationMode.Virtual)
                CloseEditingPopupDelegate?.Invoke();
        }
        return Task.CompletedTask;
    }

    public async Task RowDeleteBegin(GridRowModel<T> rowModel)
    {
        await OnRowRemoving.InvokeAsync(rowModel.DataItem);
    }

    public async Task RowInsertBegin()
    {
        var insertModel = (T?)Activator.CreateInstance(typeof(T));
        if (insertModel is null)
            throw new InvalidOperationException("Failed to create instance of type T");

        var rowModel = new GridRowModel<T>(insertModel, _columnService.DataColumnModels, 0, _repositoryService.PropInfo, false);
        rowModel.CreateEditingModel();

        await OnRowInsertBegin.InvokeAsync();

        if (EditMode.Value == GridEditMode.Popup)
            ShowInsertingPopupDelegate?.Invoke(rowModel);
        else if (EditMode.Value == GridEditMode.Form)
            ShowInsertingFormDelegate?.Invoke(rowModel);

        _stateService.DataEditState.OnNext(DataEditState.Inserting);
    }

    public async Task RowInsertCommit(T dataItem)
    {
        await OnRowInserting.InvokeAsync(dataItem);

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

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
