namespace Samovar.Grid;

public partial class SelectionColumnModel
    : DeclarativeColumnModel
{
    public override ColumnType ColumnType { get; } = ColumnType.SelectionColumn;

    public SelectionColumnModel()
        : base()
    {
        DeclaratedWidthParameter.OnNext("30px");
        MinWidth = 30;
    }
}
