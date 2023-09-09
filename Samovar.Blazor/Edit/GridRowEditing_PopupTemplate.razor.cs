using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Samovar.Blazor.Edit
{
    public partial class GridRowEditing_PopupTemplate<TItem>
        : SmDesignComponentBase, IDisposable
    {
        [SmInject]
        public IEditingService<TItem> EditingService { get; set; }

        [SmInject]
        public IJsService JsService { get; set; }

        [Parameter]
        public RenderFragment<TItem> Template { get; set; }

        [Parameter]
        public SmDataGridRowModel<TItem> RowModel { get; set; }

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
