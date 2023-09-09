using Microsoft.JSInterop;

namespace Samovar.Blazor
{
    public interface IColumnResizingService
    {
        //object Data { get; set; }
        //string Zone { get; set; }

        //void StartDrag(object data, string zone);

        //bool Accepts(string zone);

        DotNetObjectReference<IColumnResizingService> ColumnResizingDotNetRef { get; }


        bool IsMouseDown { get; set; }
        double StartMouseMoveX { get; set; }
        double EndMouseMoveX { get; set; }
        double OldAbsoluteVisibleWidthValue { get; set; }
        double OldAbsoluteEmptyColVisibleWidthValue { get; set; }
        IDataColumnModel MouseMoveCol { get; set; }

        void Reset();
        
    }
}
