namespace Samovar.Grid
{
    public class EmptyColumnModel
        : ColumnModel
    {
        public override ColumnType ColumnType { get; } = ColumnType.EmptyColumn;
    }
}
