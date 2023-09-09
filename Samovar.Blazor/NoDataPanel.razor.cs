using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public partial class NoDataPanel
        : SmDesignComponentBase, IDisposable
    {
        //[CascadingParameter(Name = "datagrid-container")]
        //protected SamovarGrid<TItem> DataGrid { get; set; }

        //[CascadingParameter(Name = "datagrid-gridcolumnservice")]
        //GridColumnService GridColumnService { get; set; }
        [SmInject]
        IJsService JsService { get; set; }

        [SmInject]
        IConstantService ConstantService { get; set; }

        [SmInject]
        public IColumnService ColumnService { get; set; }


        protected double ContainerHeight = 0;
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                ContainerHeight = 330;// await JsService.GetInnerGridHeight();// await (await JsService.JsModule()). InvokeAsync<double>("getElementHeight", new[] { ConstantService.InnerGridId });

            //return base.OnAfterRenderAsync(firstRender);
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
