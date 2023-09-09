using Microsoft.JSInterop;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public class ColumnResizingService
        : IColumnResizingService, IDisposable
    {
        private IJsService _jsService;
        private IColumnService _columnService;
        private ILayoutService _layoutService;

        public ColumnResizingService(
            IJsService jsService, 
            IColumnService columnService, 
            ILayoutService layoutService)
        {
            _jsService = jsService;
            _columnService = columnService;
            _layoutService = layoutService;
            
            ColumnResizingDotNetRef = DotNetObjectReference.Create(this as IColumnResizingService);
        }

        //public object Data { get; set; }
        //public string Zone { get; set; }

        //public void StartDrag(object data, string zone)
        //{
        //    Data = data;
        //    Zone = zone;
        //}

        //public bool Accepts(string zone)
        //{
        //    return Zone == zone;
        //}

        public void Dispose()
        {
            //TODO
        }



        public bool IsMouseDown { get; set; }
        public double StartMouseMoveX { get; set; } = double.NaN;
        public double EndMouseMoveX { get; set; } = double.NaN;
        public double OldAbsoluteVisibleWidthValue { get; set; } = double.NaN;
        public double OldAbsoluteEmptyColVisibleWidthValue { get; set; } = double.NaN;
        public IDataColumnModel MouseMoveCol { get; set; } = null;

        public DotNetObjectReference<IColumnResizingService> ColumnResizingDotNetRef { get; private set; }

        public void Reset()
        {
            IsMouseDown = false;
            StartMouseMoveX = double.NaN;
            EndMouseMoveX = double.NaN;
            OldAbsoluteVisibleWidthValue = double.NaN;
            OldAbsoluteEmptyColVisibleWidthValue = double.NaN;
        }

        [JSInvokable]
        public async Task Js_Window_MouseUp(string colMetaId, double newVisibleAbsoluteWidthValue, string emptyColumnId, double emptyColWidth)
        {
            await _jsService.DetachWindowMouseMoveEvent();
            await _jsService.DetachWindowMouseUpEvent();

            if (!string.IsNullOrEmpty(colMetaId)) {
                var col = _columnService.AllColumnModels.FirstOrDefault(c => c.Id == colMetaId);
                if(col != default(IColumnModel))
                    col.VisibleAbsoluteWidthValue = newVisibleAbsoluteWidthValue;
            }

            _columnService.EmptyColumnModel.VisibleAbsoluteWidthValue = emptyColWidth;

            Reset();

            if (!_layoutService.FitColumnsToTableWidth)
            {
                var newGridColWidthSum = _columnService.AllColumnModels.Sum(c => c.VisibleAbsoluteWidthValue);

                _layoutService.GridColWidthSum = newGridColWidthSum;
            }
        }
    }
}
