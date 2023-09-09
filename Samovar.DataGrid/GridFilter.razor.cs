using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;

namespace Samovar.DataGrid
{
    public partial class GridFilter<TItem>
        : IDisposable
    {
        [CascadingParameter(Name = "datagrid-container")]
        protected SamovarGrid<TItem> DataGrid { get; set; }

        [CascadingParameter(Name = "datagrid-gridcolumnservice")]
        protected GridColumnService GridColumnService { get; set; }

        [Inject]
        protected IJSRuntime JsRuntime { get; set; }
        
        protected readonly List<Type> Numeric_Types_For_Constant_Expression = new List<Type>
        
                {
                    typeof(byte),
                    typeof(byte?),
                    typeof(sbyte),
                    typeof(sbyte?),
                    typeof(short),
                    typeof(short?),
                    typeof(ushort),
                    typeof(ushort?),
                    typeof(int),
                    typeof(int?),
                    typeof(uint),
                    typeof(uint?),
                    typeof(long),
                    typeof(long?),
                    typeof(ulong),
                    typeof(ulong?),
                    typeof(float),
                    typeof(float?),
                    typeof(double),
                    typeof(double?),
                    typeof(decimal),
                    typeof(decimal?),
            };

        #region Column width per MouseMove

        #endregion
        public void Dispose()
        {
            //this.grid.NotifierService.NotifyAfterScroll -= NotifierService_NotifyAfterScroll;
        }
    }
}
