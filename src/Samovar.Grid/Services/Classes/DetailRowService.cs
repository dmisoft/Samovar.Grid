namespace Samovar.Grid;

public class DetailRowService<T>
    : IDetailRowService<T>
{
    public DetailRowService(ILayoutService layoutService)
    {
        layoutService.ShowDetailRow.Subscribe(async showDetailRow =>
        {
            if (!showDetailRow)
                await CollapseAllDetailRows();
        });
    }
    public List<GridRowModel<T>> ExpandedGridRows { get; } = [];

    public Task ExpandOrCollapseDetailRow(GridRowModel<T> dataItem)
    {
        if (!ExpandedGridRows.Contains(dataItem))
            ExpandedGridRows.Add(dataItem);
        else
            ExpandedGridRows.Remove(dataItem);

        return Task.CompletedTask;
    }

    public Task CollapseAllDetailRows()
    {
        ExpandedGridRows.ForEach(row => row.RowDetailExpanded = false);
        ExpandedGridRows.Clear();
        return Task.CompletedTask;
    }
}
