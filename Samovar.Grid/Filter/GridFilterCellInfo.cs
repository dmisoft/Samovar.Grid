namespace Samovar.Grid.Filter;

public sealed class GridFilterCellInfo
    : IEquatable<GridFilterCellInfo>
{
    public IDataColumnModel? ColumnModel { get; set; }
    public object? FilterCellValue { get; set; }
    public byte FilterCellMode { get; set; }

    public bool Equals(GridFilterCellInfo? other)
    {
        if (ColumnModel is null) return false;
        if (other == null) return false;
        if (other.ColumnModel is null) return false;

        return ColumnModel.Field.Equals(other.ColumnModel.Field);
    }
}
