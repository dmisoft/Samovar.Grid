using ClosedXML.Excel;
using System.Text;

namespace Samovar.Grid;

public class ExportService<T>(IColumnService columnService, IRepositoryService<T> repositoryService)
    : IExportService<T>
{
    public Task<byte[]> ExportToExcelAsync(IEnumerable<T> data)
    {
        var columns = columnService.DataColumnModels.ToList();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Export");

        // Header row
        for (int i = 0; i < columns.Count; i++)
        {
            worksheet.Cell(1, i + 1).Value = columns[i].Title.Value;
        }

        // Data rows
        int row = 2;
        foreach (var item in data)
        {
            for (int col = 0; col < columns.Count; col++)
            {
                var field = columns[col].Field.Value;
                if (repositoryService.PropInfo.TryGetValue(field, out var propInfo))
                {
                    var value = propInfo.GetValue(item);
                    worksheet.Cell(row, col + 1).Value = value is null ? XLCellValue.FromObject(string.Empty) : XLCellValue.FromObject(value);
                }
            }
            row++;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return Task.FromResult(stream.ToArray());
    }

    public Task<byte[]> ExportToCsvAsync(IEnumerable<T> data)
    {
        var columns = columnService.DataColumnModels.ToList();
        var sb = new StringBuilder();

        // Header row
        sb.AppendLine(string.Join(",", columns.Select(c => QuoteCsvField(c.Title.Value))));

        // Data rows
        foreach (var item in data)
        {
            var values = columns.Select(col =>
            {
                var field = col.Field.Value;
                if (repositoryService.PropInfo.TryGetValue(field, out var propInfo))
                {
                    var value = propInfo.GetValue(item);
                    return QuoteCsvField(value?.ToString() ?? string.Empty);
                }
                return QuoteCsvField(string.Empty);
            });
            sb.AppendLine(string.Join(",", values));
        }

        // UTF-8 with BOM for Excel compatibility
        var preamble = Encoding.UTF8.GetPreamble();
        var content = Encoding.UTF8.GetBytes(sb.ToString());
        var result = new byte[preamble.Length + content.Length];
        preamble.CopyTo(result, 0);
        content.CopyTo(result, preamble.Length);
        return Task.FromResult(result);
    }

    private static string QuoteCsvField(string value)
        => "\"" + value.Replace("\"", "\"\"") + "\"";
}
