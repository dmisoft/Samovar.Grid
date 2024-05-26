using System.Reactive.Subjects;

namespace Samovar.Blazor;
public interface IColumnModel
{
    public string Id { get; }
    public ColumnType ColumnType { get; }
    public int Order { get; set; }
    public double Width { get; set; }
    public string HeaderCellId { get; }
    public string FilterCellId { get; }
    public string HiddenHeaderCellId { get; }
}