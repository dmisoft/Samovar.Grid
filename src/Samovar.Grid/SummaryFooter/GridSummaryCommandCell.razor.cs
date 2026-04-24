using Microsoft.AspNetCore.Components;

namespace Samovar.Grid;

public partial class GridSummaryCommandCell
    : DesignComponentBase, IAsyncDisposable
{
    [Parameter]
    public required IColumnModel ColumnModel { get; set; }

    protected string WidthStyle = "";
    private IDisposable? _widthSub;

    protected override Task OnInitializedAsync()
    {
        _widthSub = ColumnModel.WidthStyle.Subscribe(w =>
        {
            WidthStyle = w;
            _ = InvokeAsync(StateHasChanged);
        });
        return base.OnInitializedAsync();
    }

    public ValueTask DisposeAsync()
    {
        _widthSub?.Dispose();
        return ValueTask.CompletedTask;
    }
}
