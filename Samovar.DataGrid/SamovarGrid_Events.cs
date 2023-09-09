using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Samovar.DataGrid
{
    public partial class SamovarGrid<TItem>
    {
        /***
         * Row selected
         */
        [Parameter]
        public Action<TItem> RowSelected { get; set; }
        [Parameter]
        public Func<TItem, Task> RowSelectedAsync { get; set; }

        /***
         * Row double click
         */
        [Parameter]
        public Action<TItem> RowDoubleClick { set; get; }
        [Parameter]
        public Func<TItem, Task> RowDoubleClickAsync { set; get; }

        /***
         * Inserting
         */
        [Parameter]
        public Action<TItem> RowInserting { get; set; }
        [Parameter]
        public Func<TItem, Task> RowInsertingAsync { get; set; }

        //[Parameter]
        //public Action<TItem> RowInsertBegin { get; set; }

        [Parameter]
        public EventCallback RowInsertBegin { get; set; }

        [Parameter]
        public Func<TItem, Task> RowInsertBeginAsync { get; set; }

        [Parameter]
        public Action RowInsertingCancel { get; set; }
        [Parameter]
        public Func<Task> RowInsertingCancelAsync { get; set; }

        //[Parameter]
        //public Action<TItem> RowInsertingCommit { get; set; }
        //[Parameter]
        //public Func<TItem, Task> RowInsertingCommitAsync { get; set; }


        /***
         * Editing
         */
        [Parameter]
        public EventCallback<TItem> RowEditBegin { set; get; }

        [Parameter]
        public EventCallback RowEditCancel { set; get; }

        [Parameter]
        public EventCallback<GridRowEditEventArgs<TItem>> RowEditCommit { set; get; }

        /***
         * Deleting
         */
        [Parameter]
        public Action<TItem> RowDeleting { get; set; }
        [Parameter]
        public Func<TItem, Task> RowDeletingAsync { get; set; }

        /***
         * Filter events
         */
        [Parameter]
        public EventCallback<GridFilterEventArgs> AfterFilter { get; set; }

        //Sort events
        [Parameter]
        public EventCallback<GridSortEventArgs> AfterSort { get; set; }

        //Paging events
        [Parameter]
        public EventCallback<GridPagingEventArgs> AfterPageChange { get; set; }

        internal void FireRowSelected(TItem rowData)
        {
            if (RowSelectedAsync != null)
                RowSelectedAsync.Invoke(rowData);
            else
                RowSelected?.Invoke(rowData);
        }

        internal void FireRowDoubleClick(GridRowEventArgs args, GridRowModel<TItem> mainModel)
        {
            if (RowDoubleClickAsync != null)
                RowDoubleClickAsync.Invoke(mainModel.dataItem);
            else
                RowDoubleClick?.Invoke(mainModel.dataItem);
        }
    }
}
