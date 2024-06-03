using System.Reactive.Subjects;

namespace Samovar.Grid
{
    public interface IDataSourceService<T>
    {
        BehaviorSubject<IEnumerable<T>> Data { get; }
        BehaviorSubject<IQueryable<T>?> DataQuery { get; }
    }
}
