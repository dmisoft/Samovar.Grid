using Microsoft.AspNetCore.Components;
using System;

namespace Samovar.Blazor.Edit
{
    public partial class GridRowInserting_Form<TItem>
        : SmDesignComponentBase, IDisposable
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [SmInject]
        public ILayoutService LayoutService { get; set; }

        [SmInject]
        public IColumnService ColumnService { get; set; }

        [SmInject]
        public IEditingService<TItem> EditingService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [Parameter]
        public SmDataGridRowModel<TItem> RowModel { get; set; }

        public void Dispose()
        {
            GC.Collect();
        }
    }
}
