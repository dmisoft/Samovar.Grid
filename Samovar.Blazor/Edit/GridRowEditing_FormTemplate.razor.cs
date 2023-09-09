using Microsoft.AspNetCore.Components;
using System;

namespace Samovar.Blazor.Edit
{
    public partial class GridRowEditing_FormTemplate<TItem>
        : SmDesignComponentBase, IDisposable
    {
        [SmInject]
        public ILayoutService LayoutService { get; set; }

        [SmInject]
        public IColumnService ColumnService { get; set; }

        [SmInject]
        public ITemplateService<TItem> TemplateService { get; set; }

        [Parameter]
        public SmDataGridRowModel<TItem> RowModel { get; set; }

        public void Dispose()
        {
            GC.Collect();
        }
    }
}
