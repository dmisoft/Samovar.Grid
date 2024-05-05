using Microsoft.JSInterop;

namespace Samovar.Blazor
{
    public interface IColumnResizingService
    {
        DotNetObjectReference<IColumnResizingService> ColumnResizingDotNetRef { get; }

        bool IsMouseDown { get; set; }
        double StartMouseMoveX { get; set; }
        double EndMouseMoveX { get; set; }
        double OldAbsoluteVisibleWidthValue { get; set; }
        double OldAbsoluteEmptyColVisibleWidthValue { get; set; }
        IDataColumnModel? MouseMoveCol { get; set; }

        void Reset();
    }
}
