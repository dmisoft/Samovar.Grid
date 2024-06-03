using Microsoft.JSInterop;
using System.Reactive.Subjects;

namespace Samovar.Grid;
public interface IColumnResizingService
{
    DotNetObjectReference<IColumnResizingService> ColumnResizingDotNetRef { get; }

    bool IsMouseDown { get; set; }
    double StartMouseMoveX { get; set; }
    double EndMouseMoveX { get; set; }
    IDataColumnModel? MouseMoveCol { get; set; }
}
