using System.Reflection;

namespace Samovar.Grid;

public class DataGridRowCellModel<T>(T? rowData, PropertyInfo pi, IDataColumnModel columnMetadata)
{
    internal string CellValue { get; private set; } = rowData is not null ? (pi.GetValue(rowData)?.ToString() ?? "") : string.Empty;
    public IDataColumnModel ColumnMetadata { get; set; } = columnMetadata;
    public string Style { get; set; } = string.Empty;
    public PropertyInfo Pi { get; private set; } = pi;
}
