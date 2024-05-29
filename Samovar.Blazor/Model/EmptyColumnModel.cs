namespace Samovar.Blazor
{
    public class EmptyColumnModel
        : ColumnModel
    {
        public override ColumnType ColumnType { get; } = ColumnType.EmptyColumn;
    }
}
