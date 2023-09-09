using Microsoft.AspNetCore.Components;
using System;

namespace Samovar.Blazor.Edit
{
    public partial class GridRowEditing_Body<TItem>
        : SmDesignComponentBase, IDisposable
    {

        [Parameter]
        public SmDataGridRowModel<TItem> RowModel { get; set; }

        public void Dispose()
        {
            GC.Collect();
        }
    }
}
