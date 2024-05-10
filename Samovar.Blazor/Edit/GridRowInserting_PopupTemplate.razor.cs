using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Samovar.Blazor.Edit
{
    public partial class GridRowInserting_PopupTemplate<T>
        : SmDesignComponentBase, IAsyncDisposable
    {
        [SmInject]
        public required  IEditingService<T> EditingService { get; set; }

        [SmInject]
        public required IJsService JsService { get; set; }

        [Parameter]
        public required RenderFragment<T> Template { get; set; }

        [Parameter]
        public required SmDataGridRowModel<T> RowModel { get; set; }

        protected string Id { get; set; } = Guid.NewGuid().ToString().Replace("-", "");

        protected ElementReference Ref { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                await (await JsService.JsModule()).InvokeVoidAsync("dragElement", Ref);
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
