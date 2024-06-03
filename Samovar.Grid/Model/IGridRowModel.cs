namespace Samovar.Grid;

public interface IGridRowModel<T>
: IComponentModel
{
	public T DataItem { get; set; }
	public T? EditingDataItem { get; set; }
}
