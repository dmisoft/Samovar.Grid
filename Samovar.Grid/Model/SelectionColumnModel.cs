namespace Samovar.Grid;

public partial class CheckboxColumnModel
    : DeclarativeColumnModel
{
    public override ColumnType ColumnType { get; } = ColumnType.CheckboxColumn;

    public CheckboxColumnModel()
        : base()
    {
        DeclaratedWidthParameter.OnNext("30px");
    }
}
