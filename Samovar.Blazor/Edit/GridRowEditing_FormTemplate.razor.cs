﻿using Microsoft.AspNetCore.Components;
using System;

namespace Samovar.Blazor.Edit
{
    public partial class GridRowEditing_FormTemplate<TItem>
        : SmDesignComponentBase, IAsyncDisposable
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [SmInject]
        public ILayoutService LayoutService { get; set; }

        [SmInject]
        public IColumnService ColumnService { get; set; }

        [SmInject]
        public ITemplateService<TItem> TemplateService { get; set; }

        [Parameter]
        public SmDataGridRowModel<TItem> RowModel { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
