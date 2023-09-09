using System.Linq.Expressions;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Samovar.Blazor
{
    public class DataGridRowCellModel<T>
    {
        internal string CellValue { get; private set; } = "...";
        public IDataColumnModel ColumnMetadata { get; set; }
        public string Style { get; set; }
        public PropertyInfo Pi { get; private set; }
        private T _underlyingRowData;// { get; private set; }
        public DataGridRowCellModel(T rowData, PropertyInfo pi, IDataColumnModel columnMetadata)
        {
            _underlyingRowData = rowData;
            ColumnMetadata = columnMetadata;
            Pi = pi;
            if (_underlyingRowData != null)
            {
                CellValue = Pi.GetValue(_underlyingRowData)?.ToString();
                //Expression propertyExpr = Expression.Property(Expression.Constant(_underlyingRowData),pi.Name);
                //CellValue = Expression.Lambda<Func<dynamic>>(propertyExpr).Compile()?.ToString();
            }
        }

        public DataGridRowCellModel(T rowData, IDataColumnModel columnMetadata)
        {
            //TODO dieser ctor gilt nur für CellTemplate
            //andere Lösung bitte, weil nicht übersichtlich
            _underlyingRowData = rowData;
            CellValue = "";
            ColumnMetadata = columnMetadata;
        }
    }
}
