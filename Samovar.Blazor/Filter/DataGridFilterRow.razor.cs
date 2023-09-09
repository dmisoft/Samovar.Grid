using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;

namespace Samovar.Blazor.Filter
{
    public partial class DataGridFilterRow<TItem>
        : SmDesignComponentBase, IDisposable
    {
        //[CascadingParameter(Name = "datagrid-container")]
        //protected SamovarGrid<TItem> DataGrid { get; set; }

        //[CascadingParameter(Name = "datagrid-gridcolumnservice")]
        //protected GridColumnService GridColumnService { get; set; }

        //[Inject]
        //protected IJSRuntime JsRuntime { get; set; }
        [SmInject]
        public IColumnService ColumnService { get; set; }
        
        [SmInject]
        public ILayoutService LayoutService { get; set; }

        [SmInject]
        public IFilterService FilterService { get; set; }

        [SmInject]
        public IRepositoryService<TItem> RepositoryService { get; set; }
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

        //#region Column width per MouseMove

        //#endregion
        public void Dispose()
        {
            //this.grid.NotifierService.NotifyAfterScroll -= NotifierService_NotifyAfterScroll;
        }
    }
}
