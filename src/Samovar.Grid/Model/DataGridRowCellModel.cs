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
        
        var format = ColumnMetadata.Format.Value;
        
        if (value is IFormattable formattable)
        {
            try
            {
                return formattable.ToString(format, CultureInfo.CurrentCulture);
            }
            catch (FormatException)
            {
                return value.ToString() ?? string.Empty;
            }
        }
        
        return value.ToString() ?? string.Empty;
    }
}
