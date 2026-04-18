using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Samovar.Grid.Edit;

public partial class GridRowInserting_Popup<T>
    : DesignComponentBase, IAsyncDisposable
{
    [SmInject]
    public required IEditingService<T> EditingService { get; set; }

    [SmInject]
    public required IJsService JsService { get; set; }

    [Parameter]
    public required GridRowModel<T> RowModel { get; set; }

    protected string Id { get; set; } = Guid.NewGuid().ToString().Replace("-", "");

    protected ElementReference ElementReference { get; set; }

    private IDisposable? _cultureSubscription;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await (await JsService.JsModule()).InvokeVoidAsync("dragElement", ElementReference);
    }

    protected override Task OnInitializedAsync()
    {
        _cultureSubscription = L10n.CultureChanged.Subscribe(u => InvokeAsync(StateHasChanged));
        return base.OnInitializedAsync();
    }

    public ValueTask DisposeAsync()
    {
        _cultureSubscription?.Dispose();
        _cultureSubscription = null;
        return ValueTask.CompletedTask;
    }
}
