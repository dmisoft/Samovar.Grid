using Microsoft.AspNetCore.Components;

namespace Samovar.Grid.Filter;

public partial class GridFilterExpressionCell
    : DesignComponentBase, IDisposable
{
    [Parameter]
    public required IDataColumnModel ColumnModel { get; set; }

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

    public void Dispose() => _widthSub?.Dispose();
}
