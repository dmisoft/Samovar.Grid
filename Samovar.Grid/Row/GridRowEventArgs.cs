namespace Samovar.Grid;

public class GridRowEventArgs(object? rowData, int rowPosition)
    : EventArgs
{
    public object RowData { get; private set; } = rowData ?? throw new ArgumentNullException(nameof(rowData));
    public int RowPosition { get; private set; } = rowPosition;
}
