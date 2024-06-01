using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public interface ISortingService
    {
        BehaviorSubject<ColumnOrderInfo> ColumnOrderInfo { get; }
        Task OnColumnClick(IDataColumnModel columnModel);
    }
}
