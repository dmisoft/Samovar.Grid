using Microsoft.AspNetCore.Components;
using System;

namespace Samovar.DataGrid
{
    public partial class GridFilterHeaderScrollbar<TItem>
        : IDisposable
    {
        [CascadingParameter(Name = "datagrid-container")]
        protected SamovarGrid<TItem> grid { get; set; }
        
        public void Dispose()
        {
        }
    }
}
