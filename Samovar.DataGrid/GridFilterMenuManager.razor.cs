using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;

namespace Samovar.DataGrid
{
    public partial class GridFilterMenuManager<TItem>
        : IDisposable
    {
        [CascadingParameter(Name = "datagrid-container")]
        protected SamovarGrid<TItem> DataGrid { get; set; }
        
        [CascadingParameter(Name = "datagrid-gridcolumnservice")]
        protected GridColumnService GridColumnService { get; set; }

        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
