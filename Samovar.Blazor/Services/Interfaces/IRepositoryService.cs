using Microsoft.AspNetCore.Components;
using System.Reactive.Subjects;
using System.Reflection;

namespace Samovar.Blazor
{
    public interface IRepositoryService<T>
    {
        IEnumerable<SmDataGridRowModel<T>> ViewCollection { get; }
        BehaviorSubject<HashSet<T>> Data { get; }
        public Dictionary<string, PropertyInfo> PropInfo { get; }
        List<EventCallback<IEnumerable<SmDataGridRowModel<T>>>> CollectionViewChangedEvList { get; set; }
    }
}
