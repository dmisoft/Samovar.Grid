using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Samovar.Blazor.Edit
{
    public partial class GridRowInserting_Popup<T>
        : SmDesignComponentBase, IDisposable
    {
        [SmInject]
        protected IEditingService<T> EditingService { get; set; }

        [Parameter]
        public SmDataGridRowModel<T> RowModel { get; set; }

        [SmInject]
        public IJsService JsService { get; set; }

        protected string Id { get; set; } = Guid.NewGuid().ToString().Replace("-", "");

        protected ElementReference Ref { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                await (await JsService.JsModule()).InvokeVoidAsync("dragElement", Ref);
        }

        public void Dispose()
        {
            GC.Collect();
        }
    }
}
