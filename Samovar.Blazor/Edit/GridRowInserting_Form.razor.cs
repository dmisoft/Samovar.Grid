using Microsoft.AspNetCore.Components;
using System;

namespace Samovar.Blazor.Edit
{
    public partial class GridRowInserting_Form<TItem>
        : SmDesignComponentBase, IDisposable
    {
        [SmInject]
        public ILayoutService LayoutService { get; set; }

        [SmInject]
        public IColumnService ColumnService { get; set; }

        [SmInject]
        public IEditingService<TItem> EditingService { get; set; }

        [Parameter]
        public SmDataGridRowModel<TItem> RowModel { get; set; }

        public void Dispose()
        {
            GC.Collect();
        }
    }
}
