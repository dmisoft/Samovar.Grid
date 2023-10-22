using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public interface IDataSourceService<T>
    {
        ISubject<IEnumerable<T>> Data { get; }
        ISubject<IQueryable<T>> DataQuery { get; }
        ISubject<NavigationStrategyDataLoadingSettings> DataLoadingSettings { get; }
    }
}
