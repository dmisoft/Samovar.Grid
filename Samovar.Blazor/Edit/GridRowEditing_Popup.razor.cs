using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Samovar.Blazor.Edit
{
    public partial class GridRowEditing_Popup<TItem>
        : SmDesignComponentBase, IDisposable
    {
        [CascadingParameter(Name = "datagrid-row")]
        protected SmDataGridRow<TItem> GridRow { get; set; }

        [SmInject]
        public IEditingService<TItem> EditingService { get; set; }

        [SmInject]
        public IJsService JsService { get; set; }

        [Parameter]
        public SmDataGridRowModel<TItem> RowModel { get; set; }

        protected ElementReference Ref { get; set; }

        protected string Id { get; set; } = Guid.NewGuid().ToString().Replace("-", "");

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                await (await JsService.JsModule()).InvokeVoidAsync("dragElement", Ref);
        }

        protected override void OnInitialized()
        {
            RowModel.CreateEditingModel();
            //if (RowModel == null)
            //{
            //    RowModel = new GridRowModel<TItem>((TItem)Activator.CreateInstance(typeof(TItem)), DataGrid.rx.GridModelService.ColumnMetadataList,
            //        0, DataGrid.rx.GridModelService.PropInfo);
            //    RowModel.RowModel = new GridRowInnerModel<TItem>
            //    {
            //        Data = RowModel.dataItem,
            //        GridCellModelCollection = RowModel.CreateGridRowCellModelCollection2(RowModel.ColumnMetadata, RowModel.PropInfo, RowModel.dataItem)
            //    };
            //    RowModel.IsLoaded = true;

            //    await DataGrid.GridEditingService.RowInsertingBegin(RowModel);
            //}
        }

        public void Dispose()
        {
            GC.Collect();
        }
    }
}
