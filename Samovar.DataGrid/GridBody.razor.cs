using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Samovar.DataGrid
{
    public partial class GridBody<TItem>
    {
        [CascadingParameter(Name = "datagrid-container")]
        public SamovarGrid<TItem> DataGrid { get; set; }
        [CascadingParameter(Name = "datagrid-gridcolumnservice")]
        protected GridColumnService GridColumnService { get; set; }

        public ElementReference GridBodyRef;
    }
}
