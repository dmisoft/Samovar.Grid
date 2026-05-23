namespace Samovar.Grid;

public class GridGroupRowModel<T>
{
    public required string Field      { get; init; }
    public required string Title      { get; init; }
    public required string GroupValue { get; set; }
    public int  GroupLevel            { get; init; }
    public bool IsExpanded            { get; set; } = true;

    public GridGroupRowModel<T>?      ParentGroup { get; init; }
    public List<GridGroupRowModel<T>> SubGroups   { get; } = [];
    public List<GridRowModel<T>>      ChildRows   { get; } = [];
}
