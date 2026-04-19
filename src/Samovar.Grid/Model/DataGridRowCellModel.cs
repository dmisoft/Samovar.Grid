using System.Globalization;
using System.Reflection;

namespace Samovar.Grid;

public class DataGridRowCellModel<T>
{
    private readonly T? _rowData;
    private readonly Func<T?, object?> _getter;

    public IDataColumnModel ColumnMetadata { get; set; }
    public PropertyInfo? Pi { get; }

    public DataGridRowCellModel(T? rowData, PropertyInfo pi, IDataColumnModel columnMetadata)
    {
        _rowData = rowData;
        Pi = pi;
        _getter = r => r is null ? null : pi.GetValue(r);
        ColumnMetadata = columnMetadata;
    }

    public DataGridRowCellModel(T? rowData, Func<T?, object?> getter, IDataColumnModel columnMetadata)
    {
        _rowData = rowData;
        Pi = null;
        _getter = getter;
        ColumnMetadata = columnMetadata;
    }

    internal string GetCellValue()
    {
        if (_rowData is null)
            return string.Empty;

        var value = _getter(_rowData);
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
