using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace Samovar.Blazor
{
    public partial class SmDataGridRowVirtualScrollingPlaceHolder
        : SmDesignComponentBase, IAsyncDisposable
    {

        [SmInject]
        public required IColumnService ColumnService { get; set; }

        [SmInject]
        public required ILayoutService LayoutService { get; set; }

        [SmInject]
        public required IGridStateService GridStateService { get; set; }

        [Parameter]
        public bool TopPlaceHolder { get; set; }

        protected string Style { get; set; } = string.Empty;

        public ValueTask DisposeAsync()
        {
            return new ValueTask(Task.CompletedTask);
        }
    }
}
