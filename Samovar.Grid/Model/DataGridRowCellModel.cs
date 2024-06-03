using System.Reflection;

namespace Samovar.Grid;
public class DataGridRowCellModel<T>
{
    internal string CellValue { get; private set; } = string.Empty;
    public IDataColumnModel ColumnMetadata { get; set; }
    public string Style { get; set; } = string.Empty;
    public PropertyInfo Pi { get; private set; }

    public DataGridRowCellModel(T? rowData, PropertyInfo pi, IDataColumnModel columnMetadata)
    {
        ColumnMetadata = columnMetadata;
        Pi = pi;
        if (rowData is not null)
        {
            var cellValue = Pi.GetValue(rowData);
            if (cellValue is not null)
                CellValue = cellValue.ToString() ?? "";
        }
    }
}
