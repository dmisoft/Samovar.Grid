using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Samovar.DataGrid
{
    public partial class GridRowEditing_PopupTemplate<TItem>
        : IDisposable
    {
        [CascadingParameter(Name = "datagrid-container")]
        protected SamovarGrid<TItem> DataGrid { get; set; }

        [CascadingParameter(Name = "datagrid-gridcolumnservice")]
        protected GridColumnService GridColumnService { get; set; }

        [CascadingParameter(Name = "datagrid-row")]
        protected GridRow<TItem> GridRow { get; set; }

        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        [Parameter]
        public GridRowModel<TItem> RowModel { get; set; }

        //protected override async Task OnInitializedAsync()
        //{
        //    if (RowModel == null)
        //    {
        //        RowModel = new GridRowModel<TItem>((TItem)Activator.CreateInstance(typeof(TItem)), DataGrid.rx.GridModelService.ColumnMetadataList,
        //            0, DataGrid.rx.GridModelService.PropInfo);
        //        RowModel.RowModel = new GridRowInnerModel<TItem>
        //        {
        //            Data = RowModel.dataItem,
        //            GridCellModelCollection = RowModel.CreateGridRowCellModelCollection2(RowModel.dataItem)
        //        };
        //        RowModel.IsLoaded = true;

        //        await DataGrid.GridEditingService.RowInsertingBegin(RowModel);
        //    }
        //}

        public void Dispose()
        {
            GC.Collect();
        }
    }
}
