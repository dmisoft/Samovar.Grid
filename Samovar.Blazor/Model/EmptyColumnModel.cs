namespace Samovar.Blazor
{
    public class EmptyColumnModel
        : ColumnModelBase
    {
        public override ColumnType ColumnType { get; } = ColumnType.EmptyColumn;
    }
}
