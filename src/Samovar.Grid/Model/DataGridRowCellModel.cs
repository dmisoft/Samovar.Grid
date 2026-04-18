using System.Globalization;
using System.Reflection;

namespace Samovar.Grid;

public class DataGridRowCellModel<T>(T? rowData, PropertyInfo pi, IDataColumnModel columnMetadata)
{
    public IDataColumnModel ColumnMetadata { get; set; } = columnMetadata;
    public PropertyInfo Pi { get; private set; } = pi;

    internal string GetCellValue()
    {
        if (rowData is null)
            return string.Empty;
        var value = Pi.GetValue(rowData);
        if (value is null)
            return string.Empty;
        if (value is IFormattable formattable)
            return formattable.ToString(null, CultureInfo.CurrentCulture);
        return value.ToString() ?? string.Empty;
    }
}
