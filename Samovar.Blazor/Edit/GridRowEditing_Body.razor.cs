﻿using Microsoft.AspNetCore.Components;

namespace Samovar.Blazor.Edit
{
    public partial class GridRowEditing_Body<TItem>
        : SmDesignComponentBase, IAsyncDisposable
    {

        [Parameter]
        public required GridRowModel<TItem> RowModel { get; set; }

        public ValueTask DisposeAsync()
        {
            return new ValueTask(Task.CompletedTask);
        }
    }
}
