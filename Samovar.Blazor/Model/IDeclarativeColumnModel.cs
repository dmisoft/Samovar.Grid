namespace Samovar.Blazor;

public interface IDeclarativeColumnModel
    : IColumnModel
{
	public DeclarativeColumnWidthMode DeclaratedWidthMode { get; set; }
	public double DeclaratedWidth { get; set; }
}
