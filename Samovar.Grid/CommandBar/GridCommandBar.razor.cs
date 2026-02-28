namespace Samovar.Grid.CommandBar;

public partial class GridCommandBar<T> : DesignComponentBase
{
    [SmInject]
    public required ILayoutService LayoutService { get; set; }

    [SmInject]
    public required IExportService<T> ExportService { get; set; }

    protected string CssClass = "";

    protected async override Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        LayoutService.CssClass.Subscribe(_ => { CssClass = _; });
    }

    [SmInject]
    public required IDataSourceService<T> DataSourceService { get; set; }

    [SmInject]
    public required IGridSelectionService<T> GridSelectionService { get; set; }

    [SmInject]
    public required IJsService JsService { get; set; }

    private IEnumerable<T> GetAllRowsInGridOrder()
        => DataSourceService.DataQuery.Value?.AsEnumerable() ?? [];

    private IEnumerable<T> GetSelectedRowsInGridOrder()
    {
        HashSet<T> selected = GridSelectionService.SelectionMode.Value switch
        {
            RowSelectionMode.Multiple => new HashSet<T>(GridSelectionService.MultipleSelectedDataRows.Value ?? []),
            RowSelectionMode.Single when GridSelectionService.SingleSelectedDataRow.Value is not null
                => [GridSelectionService.SingleSelectedDataRow.Value],
            _ => []
        };

        if (selected.Count == 0)
            return [];

        return DataSourceService.DataQuery.Value?.AsEnumerable().Where(item => selected.Contains(item)) ?? [];
    }

    private async Task ExportAllExcel()
        => await RunExport(GetAllRowsInGridOrder(), "grid-export.xlsx",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ExportService.ExportToExcelAsync);

    private async Task ExportSelExcel()
        => await RunExport(GetSelectedRowsInGridOrder(), "grid-export.xlsx",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ExportService.ExportToExcelAsync);

    private async Task ExportAllCsv()
        => await RunExport(GetAllRowsInGridOrder(), "grid-export.csv", "text/csv",
            ExportService.ExportToCsvAsync);

    private async Task ExportSelCsv()
        => await RunExport(GetSelectedRowsInGridOrder(), "grid-export.csv", "text/csv",
            ExportService.ExportToCsvAsync);

    private async Task RunExport(IEnumerable<T> data, string fileName, string contentType,
        Func<IEnumerable<T>, Task<byte[]>> exportFunc)
    {
        var bytes = await exportFunc(data);
        await JsService.DownloadFileAsync(fileName, contentType, bytes);
    }
}
