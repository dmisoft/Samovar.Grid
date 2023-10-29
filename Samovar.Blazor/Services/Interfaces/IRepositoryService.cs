using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public interface IRepositoryService<T>
    {
        IEnumerable<SmDataGridRowModel<T>> ViewCollection { get; }

        BehaviorSubject<HashSet<T>> Data { get; }
        
        //BehaviorSubject<IEnumerable<SmDataGridRowModel<T>>> ViewCollectionObservable { get; }
        public Dictionary<string, PropertyInfo> PropInfo { get; }
        
        void AttachViewCollectionSubscription();
        void DetachViewCollectionSubscription();

        //Component events
        //Func<IEnumerable<SmDataGridRowModel<T>>, Task> ViewCollectionChanged { get; set; }
        List<EventCallback<IEnumerable<SmDataGridRowModel<T>>>> CollectionViewChangedEvList { get; set; }

    }
}
