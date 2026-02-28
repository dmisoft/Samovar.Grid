namespace Samovar.Grid;

public interface IExportService<T>
{
    Task<byte[]> ExportToExcelAsync(IEnumerable<T> data);
    Task<byte[]> ExportToCsvAsync(IEnumerable<T> data);
}
