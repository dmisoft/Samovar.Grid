using Microsoft.AspNetCore.Components;

namespace Samovar.Grid.Edit;

public partial class GridRowEditing_Body<TItem>
    : DesignComponentBase, IAsyncDisposable
{
    [SmInject]
    public required ILayoutService LayoutService { get; set; }

    [Parameter]
    public required GridRowModel<TItem> RowModel { get; set; }

    protected string _labelSizeClass = "";
    protected string _formControlSizeClass = "";

    private IDisposable? _cultureSubscription;

    protected override Task OnInitializedAsync()
    {
        LayoutService.SizeMode.Subscribe(mode =>
        {
            _labelSizeClass       = mode switch { GridSizeMode.Small => "small", GridSizeMode.Large => "fs-5", _ => "" };
            _formControlSizeClass = mode switch { GridSizeMode.Small => "form-control-sm", GridSizeMode.Large => "form-control-lg", _ => "" };
            _ = InvokeAsync(StateHasChanged);
        });
        _cultureSubscription = L10n.CultureChanged.Subscribe(u => InvokeAsync(StateHasChanged));
        return base.OnInitializedAsync();
    }

    public ValueTask DisposeAsync()
    {
        _cultureSubscription?.Dispose();
        _cultureSubscription = null;
        return new ValueTask(Task.CompletedTask);
    }
}
