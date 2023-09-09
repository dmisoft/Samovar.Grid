using System.Reflection;

namespace Samovar.DataGrid
{
    internal class GridRowCellModel<T>
    {
        internal string CellValue { get; private set; } = "...";
        public ColumnMetadata ColumnMetadata { get; set; }
        public string Style { get; set; }
        public PropertyInfo Pi { get; private set; }
        private T UnderlyingRowData;// { get; private set; }
        public GridRowCellModel(T rowData, PropertyInfo pi, ColumnMetadata columnMetadata)
        {
            UnderlyingRowData = rowData;
            ColumnMetadata = columnMetadata;
            Pi = pi;
            if (UnderlyingRowData != null)
            {
                CellValue = Pi.GetValue(UnderlyingRowData)?.ToString();
            }
        }

        public GridRowCellModel(T rowData, ColumnMetadata columnMetadata)
        {
            //TODO dieser ctor gilt nur für CellTemplate
            //andere Lösung bitte, weil nicht übersichtlich
            UnderlyingRowData = rowData;
            CellValue = "";
            ColumnMetadata = columnMetadata;
        }
    }
}
