namespace Samovar.Grid;

public interface IDetailRowService<T>
{
    List<GridRowModel<T>> ExpandedGridRows { get; }
    Task ExpandOrCollapseDetailRow(GridRowModel<T> dataItem);
    Task ExpandDetailRow(GridRowModel<T> dataItem);
    Task CollapseDetailRow(GridRowModel<T> dataItem);
    Task CollapseAllDetailRows();
}
