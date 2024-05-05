using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Samovar.Blazor.Edit
{
    public partial class GridRowInserting_PopupTemplate<T>
        : SmDesignComponentBase, IDisposable

    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [SmInject]
        protected IEditingService<T> EditingService { get; set; }

        [SmInject]
        public IJsService JsService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [Parameter]
        public RenderFragment<T> Template { get; set; }

        [Parameter]
        public SmDataGridRowModel<T> RowModel { get; set; }

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
