using Microsoft.AspNetCore.Components;
using System;

namespace Samovar.DataGrid
{
    public partial class GridRowInserting_FormTemplate<TItem>
        : IDisposable
    {
        [CascadingParameter(Name = "datagrid-container")]
        protected SamovarGrid<TItem> DataGrid { get; set; }

        public void Dispose()
        {
            GC.Collect();
        }
    }
}
