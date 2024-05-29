﻿using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Samovar.Blazor.Edit
{
    public partial class GridRowInserting_Popup<T>
        : SmDesignComponentBase, IAsyncDisposable
    {
        [SmInject]
        public required IEditingService<T> EditingService { get; set; }

        [SmInject]
        public required IJsService JsService { get; set; }

        [Parameter]
        public required GridRowModel<T> RowModel { get; set; }

        protected string Id { get; set; } = Guid.NewGuid().ToString().Replace("-", "");

        protected ElementReference ElementReference { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                await (await JsService.JsModule()).InvokeVoidAsync("dragElement", ElementReference);
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
