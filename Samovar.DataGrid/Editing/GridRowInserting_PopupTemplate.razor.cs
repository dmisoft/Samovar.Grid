using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Samovar.DataGrid
{
    public partial class GridRowInserting_PopupTemplate<TItem>
        : IDisposable
    {
        [CascadingParameter(Name = "datagrid-container")]
        protected SamovarGrid<TItem> DataGrid { get; set; }

        [Parameter]
        public RenderFragment Template { get; set; }
        public void Dispose()
        {
            GC.Collect();
        }
    }
}
