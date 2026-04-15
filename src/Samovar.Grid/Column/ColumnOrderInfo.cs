namespace Samovar.Grid;

public class ColumnOrderInfo
{
    public static readonly ColumnOrderInfo Empty = new ColumnOrderInfo { Field = "", Asc = true };
    public string Field { get; set; } = "";
    public bool Asc { get; set; }
}
