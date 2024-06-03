namespace Samovar.Grid;

public partial class DetailExpanderColumnModel
	: DeclarativeColumnModel
{
	public override ColumnType ColumnType { get; } = ColumnType.DetailExpanderColumn;
	
	public DetailExpanderColumnModel()
		: base()
	{
		DeclaratedWidthParameter.OnNext("30px");
    }
}
