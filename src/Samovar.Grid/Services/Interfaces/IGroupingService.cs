using System.Reactive.Subjects;

namespace Samovar.Grid;

public interface IGroupingService<T>
{
    BehaviorSubject<bool>                           IsGroupingActive  { get; }
    BehaviorSubject<IReadOnlyList<GroupDescriptor>> GroupDescriptors  { get; }
    IObservable<IEnumerable<GridViewRow<T>>>         GroupedView       { get; }

    GroupCollapseBehavior DefaultCollapseBehavior { get; set; }

    string? DraggedColumnField { get; set; }
    string? DraggedColumnTitle { get; set; }

    void InitializeFromColumns();
    void AddGroup(GroupDescriptor descriptor);
    void RemoveGroup(string field);
    void ClearGroups();
    void ToggleGroup(GridGroupRowModel<T> group);
    void CollapseAll();
    void ExpandAll();
}
