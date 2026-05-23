namespace Samovar.Grid;

public sealed class GroupIndentColumnModel : DeclarativeColumnModel
{
    public override ColumnType ColumnType { get; } = ColumnType.GroupIndent;

    public GroupIndentColumnModel()
        : base()
    {
        DeclaratedWidthParameter.OnNext("24px");
        MinWidth = 24;
    }
}
