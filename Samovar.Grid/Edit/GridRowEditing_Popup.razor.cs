using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Samovar.Grid.Edit
{
    public partial class GridRowEditing_Popup<TItem>
        : DesignComponentBase, IAsyncDisposable
    {
        [CascadingParameter(Name = "datagrid-row")]
        public required GridRow<TItem> GridRow { get; set; }

        [SmInject]
        public required IEditingService<TItem> EditingService { get; set; }

        [SmInject]
        public required ITemplateService<TItem> TemplateService { get; set; }

        [SmInject]
        public required IJsService JsService { get; set; }

        [Parameter]
        public required GridRowModel<TItem> RowModel { get; set; }

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
