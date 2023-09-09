using Microsoft.AspNetCore.Components;
using System;

namespace Samovar.DataGrid
{
    public partial class GridHiddenHeader<TItem>
        : IDisposable
    {
        [CascadingParameter(Name = "datagrid-container")]
        protected SamovarGrid<TItem> grid { get; set; }

        [CascadingParameter(Name = "datagrid-gridcolumnservice")]
        protected GridColumnService GridColumnService { get; set; }

        public void Dispose()
        {
            //this.grid.NotifierService.NotifyAfterScroll -= NotifierService_NotifyAfterScroll;
        }
    }
}
