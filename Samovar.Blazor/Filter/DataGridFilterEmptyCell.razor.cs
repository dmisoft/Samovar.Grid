﻿using Microsoft.AspNetCore.Components;
using System.Reactive.Linq;

namespace Samovar.Blazor.Filter;

public partial class DataGridFilterEmptyCell
		: SmDesignComponentBase, IAsyncDisposable
{
    [Parameter]
    public required IColumnModel ColumnModel { get; set; }

    [SmInject]
    public required ILayoutService LayoutService { get; set; }

    [SmInject]
    public required IColumnResizingService ColumnResizingService { get; set; }

    [SmInject]
    public required IJsService JsService { get; set; }

    [SmInject]
    public required IColumnService ColumnService { get; set; }

    [SmInject]
    public required IConstantService ConstantService { get; set; }

    protected string WidthStyle = "";

    protected override Task OnInitializedAsync()
    {
        //ColumnService.ColumnResizingEndedObservable.Where(c => c.Id == Model.Id).Subscribe(c => StateHasChanged());
        ColumnModel.WidthStyle.Subscribe(w => {
            WidthStyle = w;
        });
        return base.OnInitializedAsync();
    }
   
    protected string ColumnCellDraggable = "false";

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
