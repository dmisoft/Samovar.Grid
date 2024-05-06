namespace Samovar.Blazor.Filter
{
    public sealed class DataGridFilterCellInfo
        : IEquatable<DataGridFilterCellInfo>
    {
        public IDataColumnModel? ColumnMetadata { get; set; }
        public object FilterCellValue { get; set; } = string.Empty;
        public byte FilterCellMode { get; set; }

        public bool Equals(DataGridFilterCellInfo? other)
        {
            if (ColumnMetadata is null) return false;
            if (other == null) return false;
            if (other.ColumnMetadata is null) return false;

            return ColumnMetadata.Field.Equals(other.ColumnMetadata.Field);
        }
    }
}
