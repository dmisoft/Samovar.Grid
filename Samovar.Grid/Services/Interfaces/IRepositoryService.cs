using System.Reflection;

namespace Samovar.Grid;

public interface IRepositoryService<T>
{
    public Dictionary<string, PropertyInfo> PropInfo { get; }
    IObservable<Task<IEnumerable<GridRowModel<T>>>> ViewCollectionObservableTask { get; set; }
}
