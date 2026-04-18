using Microsoft.AspNetCore.Components;

namespace Samovar.Grid.Edit;

public partial class GridRowEditing_Form<TItem>
    : DesignComponentBase, IAsyncDisposable
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [SmInject]
    public ILayoutService LayoutService { get; set; }

    [SmInject]
    public IColumnService ColumnService { get; set; }

    [SmInject]
    public IEditingService<TItem> EditingService { get; set; }

    [Parameter]
    public GridRowModel<TItem> RowModel { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    protected string _btnSizeClass = "";

    private IDisposable? _cultureSubscription;

    protected override Task OnInitializedAsync()
    {
        LayoutService.SizeMode.Subscribe(mode =>
        {
            _btnSizeClass = mode switch { GridSizeMode.Small => "btn-sm small", GridSizeMode.Large => "btn-lg", _ => "" };
            _ = InvokeAsync(StateHasChanged);
        });
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
