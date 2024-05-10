using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Samovar.Blazor.Edit
{
    public partial class GridRowEditing_Popup<TItem>
        : SmDesignComponentBase, IAsyncDisposable
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [CascadingParameter(Name = "datagrid-row")]
        public required SmDataGridRow<TItem> GridRow { get; set; }

        [SmInject]
        public IEditingService<TItem> EditingService { get; set; }

        [SmInject]
        public IJsService JsService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [Parameter]
        public required SmDataGridRowModel<TItem> RowModel { get; set; }

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
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
