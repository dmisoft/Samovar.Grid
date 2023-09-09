namespace Samovar.DataGrid
{
    public enum FilterMenuDataTypeGroup
    {
        StringFilterMenu = 0,
        NumericFilterMenu = 1,
        DateFilterMenu = 2,
        BooleanFilterMenu = 3
    }

    public class FilterOperand
    {
        public byte FilterMode { get; set; }
        public string Operand { get; set; }
    }
}
