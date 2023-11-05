using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public interface IDataSourceService<T>
    {
        BehaviorSubject<IEnumerable<T>> Data { get; }
        IObservable<IQueryable<T>> DataQuery { get; }
    }
}
