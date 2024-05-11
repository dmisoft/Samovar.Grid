using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public interface ISortingService
    {
        BehaviorSubject<DataGridColumnOrderInfo> ColumnOrderInfo { get; }
        Task OnColumnClick(IDataColumnModel columnModel);
    }
}
