using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public interface IDataSourceService<T>
    {
        BehaviorSubject<IEnumerable<T>> Data { get; }
        BehaviorSubject<IQueryable<T>> DataQuery { get; }
        IObservable<IQueryable<T>> DataQueryObservable { get; }
        BehaviorSubject<NavigationStrategyDataLoadingSettings> DataLoadingSettings { get; }
    }
}
