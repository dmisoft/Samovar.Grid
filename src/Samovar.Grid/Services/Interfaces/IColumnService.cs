using System.Reactive.Subjects;

namespace Samovar.Grid;

public interface IColumnService
{
    public List<IColumnModel> AllColumnModels { get; }
    public IEnumerable<IDeclarativeColumnModel> DeclarativeColumnModels { get; }
    public IEnumerable<IDataColumnModel> DataColumnModels { get; }
    public IColumnModel EmptyColumnModel { get; }
    public IDeclarativeColumnModel DetailExpanderColumnModel { get; }
    public IDeclarativeColumnModel RowSelectionColumnModel { get; }
    public void RegisterColumn(IColumnModel columntModel);
    public void AutoGenerateColumns<T>();
    Subject<IColumnModel> ColumnResizingEndedObservable { get; }
}
