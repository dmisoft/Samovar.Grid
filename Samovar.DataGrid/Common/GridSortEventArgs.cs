using System;

namespace Samovar.DataGrid
{
    public class GridSortEventArgs
        : EventArgs
    {
        private string sortColumn;

        public string SortColumn
        {
            get { return sortColumn; }
        }
        public GridSortEventArgs(string sortColumn)
        : base()
        {
            this.sortColumn = sortColumn;
        }
    }
}
