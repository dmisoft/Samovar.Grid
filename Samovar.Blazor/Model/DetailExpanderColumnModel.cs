namespace Samovar.Blazor;

public partial class DetailExpanderColumnModel
	: DeclarativeColumnModel
{
	public override ColumnType ColumnType { get; } = ColumnType.DetailExpanderColumn;

	public DetailExpanderColumnModel()
		: base()
	{
	}
}
