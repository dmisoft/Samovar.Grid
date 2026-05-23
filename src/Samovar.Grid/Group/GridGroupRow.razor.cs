using Microsoft.AspNetCore.Components;

namespace Samovar.Grid;

public partial class GridGroupRow<T> : DesignComponentBase
{
    [SmInject]
    public required IGroupingService<T> GroupingService { get; set; }

    [SmInject]
    public required IColumnService ColumnService { get; set; }

    [SmInject]
    public required ILayoutService LayoutService { get; set; }

    [Parameter, EditorRequired]
    public required GridGroupRowModel<T> GroupModel { get; set; }

    protected int ColSpan =>
        ColumnService.AllColumnModels.Count(c =>
            c.ColumnType == ColumnType.Data ||
            c.ColumnType == ColumnType.Expression ||
            c.ColumnType == ColumnType.Command) + 1; // +1 for empty col

    protected int RowCount => CountLeafRows(GroupModel);

    private static int CountLeafRows(GridGroupRowModel<T> node)
    {
        if (node.SubGroups.Count > 0)
            return node.SubGroups.Sum(CountLeafRows);
        return node.ChildRows.Count;
    }

    protected void ToggleExpand() => GroupingService.ToggleGroup(GroupModel);
}
