using System;

namespace Samovar.Blazor.Filter
{
    public class DataGridFilterCellInfo
        : IEquatable<DataGridFilterCellInfo>
    {
        public IDataColumnModel ColumnMetadata { get; set; }
        public object FilterCellValue { get; set; }
        public byte FilterCellMode { get; set; }

        public bool Equals(DataGridFilterCellInfo other)
        {
            return ColumnMetadata.Field.Equals(other.ColumnMetadata.Field);
        }
    }
}
