using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;

namespace Samovar.Grid;

public class GroupingService<T>
    : IGroupingService<T>, IDisposable
{
    private readonly IDataSourceService<T> _dataSourceService;
    private readonly IColumnService _columnService;
    private readonly ILayoutService _layoutService;
    private readonly IRepositoryService<T> _repositoryService;

    // Current group tree root nodes (one per top-level group value)
    private List<GridGroupRowModel<T>> _currentTree = [];

    private readonly Subject<IEnumerable<GridViewRow<T>>> _groupedViewSubject = new();
    private IDisposable? _dataQuerySubscription;

    public BehaviorSubject<bool> IsGroupingActive { get; } = new(false);
    public BehaviorSubject<IReadOnlyList<GroupDescriptor>> GroupDescriptors { get; } = new(Array.Empty<GroupDescriptor>());
    public IObservable<IEnumerable<GridViewRow<T>>> GroupedView => _groupedViewSubject.AsObservable();

    public GroupCollapseBehavior DefaultCollapseBehavior { get; set; } = GroupCollapseBehavior.AllExpanded;

    public GroupingService(
          IDataSourceService<T> dataSourceService
        , IColumnService columnService
        , ILayoutService layoutService
        , IRepositoryService<T> repositoryService)
    {
        _dataSourceService = dataSourceService;
        _columnService = columnService;
        _layoutService = layoutService;
        _repositoryService = repositoryService;

        // Subscribe to data changes when grouping is active
        _dataQuerySubscription = _dataSourceService.DataQuery
            .Subscribe(_ => RebuildIfActive());

        // Also re-scan column GroupIndex values on initialization
        ScanColumnGroupIndexes();
    }

    private void ScanColumnGroupIndexes()
    {
        var grouped = _columnService.DataColumnModels
            .Where(c => c.GroupIndex.Value.HasValue)
            .OrderBy(c => c.GroupIndex.Value!.Value)
            .Select(c => new GroupDescriptor
            {
                Field      = c.Field.Value,
                Title      = c.Title.Value,
                GroupIndex = c.GroupIndex.Value!.Value
            })
            .ToList();

        if (grouped.Count > 0)
        {
            foreach (var desc in grouped)
                AddGroupInternal(desc, suppressRebuild: true);
            RebuildView();
        }
    }

    public void AddGroup(GroupDescriptor descriptor)
    {
        AddGroupInternal(descriptor, suppressRebuild: false);
    }

    private void AddGroupInternal(GroupDescriptor descriptor, bool suppressRebuild)
    {
        var current = GroupDescriptors.Value.ToList();
        if (current.Any(d => d.Field == descriptor.Field))
            return;

        current.Add(descriptor);
        current = current.OrderBy(d => d.GroupIndex).ToList();
        GroupDescriptors.OnNext(current);

        IsGroupingActive.OnNext(true);
        _layoutService.ActiveGroupCount.OnNext(current.Count);

        if (!suppressRebuild)
            RebuildView();
    }

    public void RemoveGroup(string field)
    {
        var current = GroupDescriptors.Value.Where(d => d.Field != field).ToList();
        GroupDescriptors.OnNext(current);

        bool active = current.Count > 0;
        IsGroupingActive.OnNext(active);
        _layoutService.ActiveGroupCount.OnNext(current.Count);

        if (active)
            RebuildView();
        else
        {
            _currentTree.Clear();
            _groupedViewSubject.OnNext([]);
        }
    }

    public void ClearGroups()
    {
        GroupDescriptors.OnNext(Array.Empty<GroupDescriptor>());
        IsGroupingActive.OnNext(false);
        _layoutService.ActiveGroupCount.OnNext(0);
        _currentTree.Clear();
        _groupedViewSubject.OnNext([]);
    }

    public void ToggleGroup(GridGroupRowModel<T> group)
    {
        group.IsExpanded = !group.IsExpanded;
        EmitFlatView(_currentTree);
    }

    public void CollapseAll()
    {
        SetAllExpanded(_currentTree, false);
        EmitFlatView(_currentTree);
    }

    public void ExpandAll()
    {
        SetAllExpanded(_currentTree, true);
        EmitFlatView(_currentTree);
    }

    private void SetAllExpanded(List<GridGroupRowModel<T>> nodes, bool expanded)
    {
        foreach (var node in nodes)
        {
            node.IsExpanded = expanded;
            SetAllExpanded(node.SubGroups, expanded);
        }
    }

    private void RebuildIfActive()
    {
        if (IsGroupingActive.Value)
            RebuildView();
    }

    private void RebuildView()
    {
        var query = _dataSourceService.DataQuery.Value;
        if (query is null)
        {
            _currentTree.Clear();
            _groupedViewSubject.OnNext([]);
            return;
        }

        var descriptors = GroupDescriptors.Value;
        if (descriptors.Count == 0)
        {
            _currentTree.Clear();
            _groupedViewSubject.OnNext([]);
            return;
        }

        var data = query.ToList();
        var newTree = BuildTree(data, descriptors, parentGroup: null, level: 0);

        // Preserve expand/collapse state from previous tree
        PreserveExpandState(_currentTree, newTree);

        // Apply default collapse behavior to newly added nodes
        if (DefaultCollapseBehavior == GroupCollapseBehavior.AllCollapsed)
            SetAllExpanded(newTree, false);

        _currentTree = newTree;
        EmitFlatView(_currentTree);
    }

    private List<GridGroupRowModel<T>> BuildTree(
        List<T> data,
        IReadOnlyList<GroupDescriptor> descriptors,
        GridGroupRowModel<T>? parentGroup,
        int level)
    {
        if (level >= descriptors.Count)
            return [];

        var descriptor = descriptors[level];
        var prop = typeof(T).GetProperty(descriptor.Field, BindingFlags.Public | BindingFlags.Instance);

        var grouped = data
            .GroupBy(item => GetGroupKey(item, prop))
            .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase);

        var result = new List<GridGroupRowModel<T>>();

        foreach (var group in grouped)
        {
            var groupModel = new GridGroupRowModel<T>
            {
                Field      = descriptor.Field,
                Title      = descriptor.Title,
                GroupValue = group.Key,
                GroupLevel = level,
                IsExpanded = DefaultCollapseBehavior == GroupCollapseBehavior.AllExpanded,
                ParentGroup = parentGroup
            };

            if (level == descriptors.Count - 1)
            {
                // Leaf level: build data row models
                int pos = 0;
                foreach (var item in group)
                {
                    pos++;
                    groupModel.ChildRows.Add(new GridRowModel<T>(
                        item,
                        _columnService.DataColumnModels,
                        pos,
                        _repositoryService.PropInfo,
                        false));
                }
            }
            else
            {
                // Intermediate level: recurse
                var subGroups = BuildTree(group.ToList(), descriptors, groupModel, level + 1);
                groupModel.SubGroups.AddRange(subGroups);
            }

            result.Add(groupModel);
        }

        return result;
    }

    private static string GetGroupKey(T item, PropertyInfo? prop)
    {
        if (prop is null)
            return string.Empty;

        var value = prop.GetValue(item);
        if (value is null)
            return string.Empty;

        // DateTime: use date only for grouping
        if (value is DateTime dt)
            return dt.Date.ToShortDateString();
        if (value is DateOnly d)
            return d.ToString();

        return value.ToString() ?? string.Empty;
    }

    private static void PreserveExpandState(
        List<GridGroupRowModel<T>> oldNodes,
        List<GridGroupRowModel<T>> newNodes)
    {
        foreach (var newNode in newNodes)
        {
            var match = oldNodes.FirstOrDefault(o =>
                o.Field == newNode.Field &&
                o.GroupValue == newNode.GroupValue &&
                o.GroupLevel == newNode.GroupLevel);

            if (match is not null)
            {
                newNode.IsExpanded = match.IsExpanded;
                PreserveExpandState(match.SubGroups, newNode.SubGroups);
            }
        }
    }

    private void EmitFlatView(List<GridGroupRowModel<T>> tree)
    {
        var flat = new List<GridViewRow<T>>();
        FlattenTree(tree, flat);
        _groupedViewSubject.OnNext(flat);
    }

    private static void FlattenTree(List<GridGroupRowModel<T>> nodes, List<GridViewRow<T>> result)
    {
        foreach (var node in nodes)
        {
            result.Add(new GridGroupViewRow<T> { GroupModel = node });

            if (!node.IsExpanded)
                continue;

            if (node.SubGroups.Count > 0)
                FlattenTree(node.SubGroups, result);
            else
                foreach (var row in node.ChildRows)
                    result.Add(new GridDataViewRow<T> { RowModel = row });
        }
    }

    public void Dispose()
    {
        _dataQuerySubscription?.Dispose();
        _dataQuerySubscription = null;
        _groupedViewSubject.Dispose();
    }
}
