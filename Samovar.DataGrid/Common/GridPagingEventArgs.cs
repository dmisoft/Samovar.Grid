using System;

namespace Samovar.DataGrid
{
    public class GridPagingEventArgs
        : EventArgs
    {
        private int newPageNumber;

        public int NewPageNumber
        {
            get { return newPageNumber; }
            set { newPageNumber = value; }
        }

        public GridPagingEventArgs(int newPageNumber)
        : base()
        {
            this.newPageNumber = newPageNumber;
        }
    }
}
