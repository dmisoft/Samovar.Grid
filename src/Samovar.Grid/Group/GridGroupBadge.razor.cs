using Microsoft.AspNetCore.Components;

namespace Samovar.Grid;

public partial class GridGroupBadge : ComponentBase
{
    [Parameter, EditorRequired]
    public required string Field { get; set; }

    [Parameter, EditorRequired]
    public required string Title { get; set; }

    [Parameter]
    public EventCallback<string> OnRemove { get; set; }

    protected Task RemoveClicked() => OnRemove.InvokeAsync(Field);
}
