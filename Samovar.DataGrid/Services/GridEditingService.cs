using System.Collections.Generic;
using System.Threading.Tasks;

namespace Samovar.DataGrid
{
    internal class GridEditingService<TItem>
    {
        SamovarGrid<TItem> DataGrid;
        GridRowModel<TItem> editingRowModel;
        internal GridState GridState { get; set; } = GridState.Idle;

        internal GridEditingService()
        {
        }

        internal void Init(SamovarGrid<TItem> dataGrid)
        {
            DataGrid = dataGrid;
        }

        internal async Task RowEditBegin(GridRowModel<TItem> rowModel, bool standardEditForm)
        {
            await DataGrid.RowEditBegin.InvokeAsync(rowModel.dataItem);
            editingRowModel = rowModel;
            editingRowModel.RowState = GridRowStatee.Editing;

            if (standardEditForm) {
                editingRowModel.CreateEditingModel();
            }

            GridState = GridState.Editing;
        }

        internal async Task RowEditCommit()
        {
            GridState = GridState.Idle;

            await DataGrid.RowEditCommit.InvokeAsync(new GridRowEditEventArgs<TItem>(editingRowModel.RowModel.Data, editingRowModel.EditingRowModel.Data));

            editingRowModel.RowState = GridRowStatee.Idle;
            editingRowModel = null;

            await DataGrid.Repaint();

            //return Task.CompletedTask;
        }

        internal async Task RowEditCancel()
        {
            GridState = GridState.Idle;

            await DataGrid.RowEditCancel.InvokeAsync(editingRowModel.dataItem);

            editingRowModel.RowState = GridRowStatee.Idle;
            editingRowModel = null;

            await DataGrid.Repaint();
        }

        //Row deleting
        internal async Task RowDeleteBegin(GridRowModel<TItem> rowModel)
        {
            editingRowModel = rowModel;
            DataGrid.RowDeleting?.Invoke(editingRowModel.dataItem);
            DataGrid.RowDeletingAsync?.Invoke(editingRowModel.dataItem);
            await DataGrid.Repaint();

            //return Task.CompletedTask;
        }

        internal async Task CancelTemplateRowEdit()
        {
            editingRowModel.RowState = GridRowStatee.Idle;
            GridState = GridState.Idle;

            //DMi TODO
            editingRowModel = null;
            await DataGrid.RowEditCancel.InvokeAsync();

            await DataGrid.Repaint();
            //return Task.CompletedTask;
        }

        internal async Task CancelTemplateRowInserting()
        {
            GridState = GridState.Idle;
            await DataGrid.Repaint();
        }

        //Row inserting
        internal async Task RowInsertingBegin(GridRowModel<TItem> rowModel)
        {
            GridState = GridState.Inserting;
            editingRowModel = rowModel;
            editingRowModel.RowState = GridRowStatee.Inserting;
            editingRowModel.CreateEditingModel();

            await DataGrid.Repaint();
        }

        internal async Task RowInsertingCommit()
        {
            GridState = GridState.Idle;
            //editingRowModel.EndEditingModel();

            DataGrid.RowInserting?.Invoke(editingRowModel.EditingRowModel.Data);
            DataGrid.RowInsertingAsync?.Invoke(editingRowModel.EditingRowModel.Data);

            editingRowModel.RowState = GridRowStatee.Idle;
            editingRowModel = null;

            await DataGrid.Repaint();

            //return Task.CompletedTask;
        }

        internal async Task RowInsertingCancel()
        {
            GridState = GridState.Idle;

            DataGrid.RowInsertingCancel?.Invoke();
            DataGrid.RowInsertingCancelAsync?.Invoke();

            editingRowModel.RowState = GridRowStatee.Idle;
            editingRowModel = null;

            await DataGrid.Repaint();
            //return Task.CompletedTask;
        }
    }
}
