namespace Samovar.Grid
{
    public class GridRowEventArgs(object? rowData, int rowPosition)
        : EventArgs
    {
        public object RowData { get; private set; } = rowData ?? throw new ArgumentNullException(nameof(rowData));
        public int RowPosition { get; private set; } = rowPosition;
    }

    public class GridRowNewEventArgs<T>(T rowData, int rowPosition)
    : EventArgs
    {
        public T RowData { get; private set; } = rowData;
        public int RowPosition { get; private set; } = rowPosition;
    }

    public class GridRowEditEventArgs<T>(T oldData, T newData)
        : EventArgs
    {
        public T OldData { get; private set; } = oldData;
        public T NewData { get; private set; } = newData;
        public int RowPosition { get; }
    }
}
