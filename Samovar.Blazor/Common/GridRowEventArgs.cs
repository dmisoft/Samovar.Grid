namespace Samovar.Blazor
{
    public class GridRowEventArgs
        : EventArgs
    {
        public object RowData { get; private set; }
        public int RowPosition { get; private set; }

        public GridRowEventArgs(object rowData, int rowPosition)
            : base()
        {
            RowData = rowData;
            RowPosition = rowPosition;
        }
    }

    public class GridRowNewEventArgs<T>
    : EventArgs
    {
        public T RowData { get; private set; }
        public int RowPosition { get; private set; }

        public GridRowNewEventArgs(T rowData, int rowPosition)
            : base()
        {
            RowData = rowData;
            RowPosition = rowPosition;
        }
    }

    public class GridRowEditEventArgs<T>
        : EventArgs
    {
        public T OldData { get; private set; }
        public T NewData { get; private set; }
        public int RowPosition { get; }

        public GridRowEditEventArgs(T oldData, T newData)
            : base()
        {
            OldData = oldData;
            NewData = newData;
        }
    }
}
