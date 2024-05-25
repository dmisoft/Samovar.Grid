using Microsoft.JSInterop;
using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public interface IColumnResizingService
    {
        DotNetObjectReference<IColumnResizingService> ColumnResizingDotNetRef { get; }

        bool IsMouseDown { get; set; }
        double StartMouseMoveX { get; set; }
        double EndMouseMoveX { get; set; }
        IDataColumnModel? MouseMoveCol { get; set; }
        Subject<IColumnModel> ColumnResizingEndedObservable { get; }
    }
}
