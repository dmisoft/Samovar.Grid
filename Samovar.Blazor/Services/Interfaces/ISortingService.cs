using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public interface ISortingService
    {
        BehaviorSubject<DataGridColumnOrderInfo> ColumnOrderInfo { get; }
        Task OnColumnClick(IDataColumnModel columnModel);
    }
}
