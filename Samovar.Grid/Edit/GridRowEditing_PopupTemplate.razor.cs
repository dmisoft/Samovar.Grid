using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Samovar.Grid.Edit
{
    public partial class GridRowEditing_PopupTemplate<TItem>
        : DesignComponentBase, IAsyncDisposable
    {
        [SmInject]
        public required IEditingService<TItem> EditingService { get; set; }

        [SmInject]
        public required IJsService JsService { get; set; }

        [Parameter]
        public required GridRowModel<TItem> RowModel { get; set; }

        [Parameter]
        public required RenderFragment<TItem> Template { get; set; }

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
