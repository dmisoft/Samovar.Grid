using Microsoft.AspNetCore.Components.Web;

namespace Samovar.Grid;

public partial class GridGroupPanel<T> : DesignComponentBase, IAsyncDisposable
{
    [SmInject]
    public required IGroupingService<T> GroupingService { get; set; }

    [SmInject]
    public required ILayoutService LayoutService { get; set; }

    protected bool IsVisible { get; private set; }
    protected bool HasGroups { get; private set; }
    protected IReadOnlyList<GroupDescriptor> Groups { get; private set; } = [];

    private IDisposable? _showSub;
    private IDisposable? _groupSub;

    protected override Task OnInitializedAsync()
    {
        _showSub = LayoutService.ShowGroupPanel.Subscribe(v =>
        {
            IsVisible = v || GroupingService.IsGroupingActive.Value;
            _ = InvokeAsync(StateHasChanged);
        });

        _groupSub = GroupingService.GroupDescriptors.Subscribe(g =>
        {
            Groups    = g;
            HasGroups = g.Count > 0;
            IsVisible = LayoutService.ShowGroupPanel.Value || HasGroups;
            _ = InvokeAsync(StateHasChanged);
        });

        return base.OnInitializedAsync();
    }

    protected void RemoveGroup(string field) => GroupingService.RemoveGroup(field);

    protected void HandleDrop(DragEventArgs e)
    {
        var field = GroupingService.DraggedColumnField;
        var title = GroupingService.DraggedColumnTitle;
        GroupingService.DraggedColumnField = null;
        GroupingService.DraggedColumnTitle = null;

        if (string.IsNullOrEmpty(field)) return;

        int nextIndex = GroupingService.GroupDescriptors.Value.Count;
        GroupingService.AddGroup(new GroupDescriptor
        {
            Field      = field,
            Title      = title ?? field,
            GroupIndex = nextIndex
        });
    }

    public ValueTask DisposeAsync()
    {
        _showSub?.Dispose();
        _groupSub?.Dispose();
        return ValueTask.CompletedTask;
    }
}
