﻿using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public interface IDataSourceService<T>
    {
        BehaviorSubject<IEnumerable<T>> Data { get; }
        Subject<IQueryable<T>> DataQuery { get; }
        BehaviorSubject<NavigationStrategyDataLoadingSettings> DataLoadingSettings { get; }
    }
}
