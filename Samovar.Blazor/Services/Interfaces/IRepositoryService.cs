using Microsoft.AspNetCore.Components;
using System.Reactive.Subjects;
using System.Reflection;

namespace Samovar.Blazor;

public interface IRepositoryService<T>
{
    IEnumerable<GridRowModel<T>> ViewCollection { get; }
    BehaviorSubject<HashSet<T>> Data { get; }
    public Dictionary<string, PropertyInfo> PropInfo { get; }
    IObservable<Task<IEnumerable<GridRowModel<T>>>> ViewCollectionObservableTask { get; set; }
}
