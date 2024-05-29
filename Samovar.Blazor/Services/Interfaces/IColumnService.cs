using System.Reactive.Subjects;

namespace Samovar.Blazor;
public interface IColumnService
{
	public List<IColumnModel> AllColumnModels { get; }
	public IEnumerable<IDeclarativeColumnModel> DeclarativeColumnModels { get; }
	public IEnumerable<IDataColumnModel> DataColumnModels { get; }
	public IColumnModel EmptyColumnModel { get; }
	public IDeclarativeColumnModel DetailExpanderColumnModel { get; }
	public void RegisterColumn(IColumnModel columntModel);
	Subject<IColumnModel> ColumnResizingEndedObservable { get; }
}
