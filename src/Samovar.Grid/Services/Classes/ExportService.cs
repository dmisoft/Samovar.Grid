using ClosedXML.Excel;
using Samovar.Grid.Formulas;
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

        for (int i = 0; i < columns.Count; i++)
        {
            worksheet.Cell(1, i + 1).Value = columns[i].Title.Value;
        }

        int row = 2;
        foreach (var item in data)
        {
            for (int col = 0; col < columns.Count; col++)
            {
                var column = columns[col];
                var value = ResolveValue(column, item);
                worksheet.Cell(row, col + 1).Value = value is null ? XLCellValue.FromObject(string.Empty) : XLCellValue.FromObject(value);
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

        sb.AppendLine(string.Join(",", columns.Select(c => QuoteCsvField(c.Title.Value))));

        foreach (var item in data)
        {
            var values = columns.Select(col =>
            {
                var value = ResolveValue(col, item);
                return QuoteCsvField(value?.ToString() ?? string.Empty);
            });
            sb.AppendLine(string.Join(",", values));
        }

        var preamble = Encoding.UTF8.GetPreamble();
        var content = Encoding.UTF8.GetBytes(sb.ToString());
        var result = new byte[preamble.Length + content.Length];
        preamble.CopyTo(result, 0);
        content.CopyTo(result, preamble.Length);
        return Task.FromResult(result);
    }

    private object? ResolveValue(IDataColumnModel column, T item)
    {
        if (column is IExpressionColumnModel expr)
        {
            var getter = (Func<T, decimal?>?)expr.CompiledGetter;
            if (getter is null)
            {
                getter = expr.Ast is null
                    ? _ => null
                    : FormulaCompiler.Compile<T>(expr.Ast, repositoryService.PropInfo);
                expr.CompiledGetter = getter;
            }
            return item is null ? null : getter(item);
        }

        var field = column.Field.Value;
        if (repositoryService.PropInfo.TryGetValue(field, out var propInfo))
            return propInfo.GetValue(item);
        return null;
    }

    private static string QuoteCsvField(string value)
        => "\"" + value.Replace("\"", "\"\"") + "\"";
}
