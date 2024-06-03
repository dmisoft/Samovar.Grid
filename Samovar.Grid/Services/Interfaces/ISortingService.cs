using System.Reactive.Subjects;

namespace Samovar.Grid
{
    public interface ISortingService
    {
        BehaviorSubject<ColumnOrderInfo> ColumnOrderInfo { get; }
        Task OnColumnClick(IDataColumnModel columnModel);
    }
}
