using System.Reactive.Subjects;

namespace Samovar.Blazor;
public interface IColumnModel
{
    public string Id { get; }
    public ColumnType ColumnType { get; }
    public int Order { get; set; }
    public BehaviorSubject<double> Width { get; }
    public string MainCellId { get; }
    public string HeaderCellId { get; }
    public string FilterCellId { get; }
    public string HiddenHeaderCellId { get; }
    public IObservable<string> WidthStyle { get; }
}