namespace Samovar.Blazor
{
    public class EmptyColumnModel
        : ColumnModelBase
    {
        public override DataGridColumnType ColumnType { get; } = DataGridColumnType.EmptyColumn;
    }
}
