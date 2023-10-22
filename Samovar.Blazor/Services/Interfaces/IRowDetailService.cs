using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
	public interface IRowDetailService<T>
    {
        BehaviorSubject<IList<T>> ExpandedRowDetails { get; }

        Task ExpandOrCloseRowDetails(T dataItem);

        Task ExpandRowDetails(T dataItem);

        Task CloseRowDetails(T dataItem);
    }
}
