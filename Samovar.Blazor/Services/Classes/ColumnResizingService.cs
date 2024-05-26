using Microsoft.JSInterop;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public class ColumnResizingService
        : IColumnResizingService, IAsyncDisposable
    {
        private readonly IJsService _jsService;
        private readonly IColumnService _columnService;
        private readonly ILayoutService _layoutService;

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

        public bool IsMouseDown { get; set; }
        public double StartMouseMoveX { get; set; } = double.NaN;
        public double EndMouseMoveX { get; set; } = double.NaN;
        public double OldAbsoluteVisibleWidthValue { get; set; } = double.NaN;
        public double OldAbsoluteEmptyColVisibleWidthValue { get; set; } = double.NaN;
        public IDataColumnModel? MouseMoveCol { get; set; }

        public DotNetObjectReference<IColumnResizingService> ColumnResizingDotNetRef { get; private set; }


        [JSInvokable]
        public async Task Js_Window_MouseUp(
            string colMetaId, 
            double newVisibleAbsoluteWidthValue, 
            string emptyColumnId, 
            double emptyColWidth, 
            string rightSideColumnId, 
            double newRightSideColumnWidth, 
            double emptyHeaderColWidth)
        {
            await _jsService.DetachWindowMouseMoveEvent();
            await _jsService.DetachWindowMouseUpEvent();

            //var emptyMainHeaderColumn = _columnService.AllColumnModels.Find(c => c.Id == _columnService.EmptyHeaderColumnModel.Id);
            //var emptyFilterHeaderColumn = _columnService.AllColumnModels.Find(c => c.Id == emptyHeaderColumnId);

			//var emptyColumn = _columnService.AllColumnModels.Find(c => c.Id == emptyColumnId);
            var col = _columnService.AllColumnModels.Find(c => c.Id == colMetaId);
            if (col != default(IColumnModel))
            {
                col.AbsoluteWidth = newVisibleAbsoluteWidthValue;
            }
            var rightSideColumn = _columnService.AllColumnModels.Find(c => c.Id == rightSideColumnId);
            if (rightSideColumn is not null)
            {
                rightSideColumn.AbsoluteWidth = newRightSideColumnWidth;
            }

            _columnService.EmptyColumnModel.AbsoluteWidth = emptyColWidth;
            _columnService.EmptyHeaderColumnModel.AbsoluteWidth = emptyHeaderColWidth;

			_columnService.ColumnResizingEndedObservable.OnNext(_columnService.EmptyColumnModel);
			_columnService.ColumnResizingEndedObservable.OnNext(_columnService.EmptyHeaderColumnModel);

			if (col is not null)
            {
                _columnService.ColumnResizingEndedObservable.OnNext(col);
            }

            if (rightSideColumn is not null)
            {
                _columnService.ColumnResizingEndedObservable.OnNext(rightSideColumn);
            }
			
			_layoutService.OriginalColumnsWidthChanged = true;
        }

        public ValueTask DisposeAsync()
        {
            ColumnResizingDotNetRef.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
