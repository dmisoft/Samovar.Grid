using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Samovar.Grid.Filter;
using System.Globalization;
using System.Reflection;

namespace Samovar.Grid.Header;

public partial class GridColumnFilterMenuPanel<TItem>
    : DesignComponentBase, IAsyncDisposable
{
    [Parameter]
    public required IDataColumnModel ColMetadata { get; set; }

    [Parameter]
    public EventCallback OnClose { get; set; }

    [Parameter]
    public double Top { get; set; }

    [Parameter]
    public double Left { get; set; }

    [SmInject]
    public required IDataSourceService<TItem> DataSourceService { get; set; }

    [SmInject]
    public required IRepositoryService<TItem> RepositoryService { get; set; }

    [SmInject]
    public required IFilterService FilterService { get; set; }

    [SmInject]
    public required IJsService JsService { get; set; }

    private ElementReference _panelRef;
    private DotNetObjectReference<GridColumnFilterMenuPanel<TItem>>? _dotNetRef;
    private bool _handlersRegistered = false;

    private List<FilterMenuTreeNode> _rootNodes = new();
    private string _searchText = "";

    private static readonly HashSet<Type> DateTypes = new()
    {
        typeof(DateTime), typeof(DateTime?),
        typeof(DateOnly), typeof(DateOnly?)
    };

    protected override Task OnInitializedAsync()
    {
        var field = ColMetadata.Field.Value;
        var rawValues = DataSourceService.GetDistinctColumnValues(field);

        PropertyInfo? propInfo = null;
        RepositoryService.PropInfo.TryGetValue(field, out propInfo);
        var propType = propInfo?.PropertyType ?? typeof(string);

        if (DateTypes.Contains(propType))
            _rootNodes = BuildDateHierarchy(rawValues, propType);
        else
            _rootNodes = BuildFlatList(rawValues);

        // If there is already an active InList filter for this column, restore checked state
        var existing = FilterService.ColumnFilters
            .FirstOrDefault(f => f.ColumnModel is not null && f.ColumnModel.Equals(ColMetadata));
        if (existing?.FilterCellMode == FilterCellModeConstants.InList && existing.FilterCellValue is IEnumerable<object> selected)
        {
            var selectedSet = selected.Select(v => v?.ToString()).ToHashSet();
            SetCheckedFromExisting(_rootNodes, selectedSet);
        }

        FilterService.FilterCleared += OnFilterCleared;
        return base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !_handlersRegistered)
        {
            _handlersRegistered = true;
            _dotNetRef = DotNetObjectReference.Create(this);
            await JsService.AddFilterMenuDismissHandlers(_panelRef, _dotNetRef);
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    [JSInvokable]
    public async Task CloseFromJs()
    {
        await OnClose.InvokeAsync();
    }

    private Task OnFilterCleared()
    {
        foreach (var node in _rootNodes)
            node.SetCheckedRecursive(true);
        StateHasChanged();
        return Task.CompletedTask;
    }

    private static List<FilterMenuTreeNode> BuildFlatList(IEnumerable<object?> values)
    {
        return values
            .Select(v => new FilterMenuTreeNode
            {
                Label = v?.ToString() ?? "(blank)",
                RawValue = v
            })
            .ToList();
    }

    private static List<FilterMenuTreeNode> BuildDateHierarchy(IEnumerable<object?> values, Type propType)
    {
        if (propType == typeof(DateOnly) || propType == typeof(DateOnly?))
        {
            var dates = values.OfType<DateOnly>().Distinct().OrderBy(d => d).ToList();
            return dates.GroupBy(d => d.Year)
                .Select(yg => new FilterMenuTreeNode
                {
                    Label = yg.Key.ToString(),
                    Children = yg.GroupBy(d => d.Month)
                        .Select(mg => new FilterMenuTreeNode
                        {
                            Label = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(mg.Key),
                            Children = mg.Select(d => new FilterMenuTreeNode
                            {
                                Label = d.Day.ToString(),
                                RawValue = d
                            }).ToList()
                        }).ToList()
                }).ToList();
        }
        else
        {
            var dates = values.OfType<DateTime>().Distinct().OrderBy(d => d).ToList();
            return dates.GroupBy(d => d.Year)
                .Select(yg => new FilterMenuTreeNode
                {
                    Label = yg.Key.ToString(),
                    Children = yg.GroupBy(d => d.Month)
                        .Select(mg => new FilterMenuTreeNode
                        {
                            Label = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(mg.Key),
                            Children = mg.Select(d => new FilterMenuTreeNode
                            {
                                Label = d.Day.ToString(),
                                RawValue = d
                            }).ToList()
                        }).ToList()
                }).ToList();
        }
    }

    private static void SetCheckedFromExisting(List<FilterMenuTreeNode> nodes, HashSet<string?> selectedLabels)
    {
        foreach (var node in nodes)
        {
            if (node.IsLeaf)
                node.IsChecked = selectedLabels.Contains(node.RawValue?.ToString());
            else
            {
                SetCheckedFromExisting(node.Children, selectedLabels);
                node.IsChecked = node.Children.All(c => c.IsChecked);
            }
        }
    }

    private IEnumerable<FilterMenuTreeNode> GetVisibleNodes()
    {
        if (string.IsNullOrEmpty(_searchText))
            return _rootNodes;
        return _rootNodes.Where(NodeMatchesSearch).ToList();
    }

    private bool NodeMatchesSearch(FilterMenuTreeNode node)
    {
        if (node.IsLeaf)
            return node.Label.Contains(_searchText, StringComparison.OrdinalIgnoreCase);
        return node.Children.Any(NodeMatchesSearch);
    }

    private bool SelectAllChecked =>
        _rootNodes.Count > 0 && _rootNodes.All(n => n.IsChecked && !n.IsIndeterminate);

    private bool SelectAllIndeterminate =>
        _rootNodes.Count > 0 && !SelectAllChecked &&
        _rootNodes.Any(n => n.IsChecked || n.IsIndeterminate);

    private void OnSelectAllChanged(ChangeEventArgs e)
    {
        bool check = e.Value is true;
        foreach (var node in GetVisibleNodes())
            node.SetCheckedRecursive(check);
    }

    private void OnNodeCheckedChanged((FilterMenuTreeNode node, ChangeEventArgs e) args)
    {
        bool check = args.e.Value is true;
        args.node.SetCheckedRecursive(check);
        // Update parent indeterminate state by refreshing from children
        foreach (var root in _rootNodes)
            root.UpdateCheckedFromChildren();
    }

    private void ToggleExpand(FilterMenuTreeNode node)
    {
        node.IsExpanded = !node.IsExpanded;
    }

    private async Task OnApply()
    {
        var selectedValues = CollectCheckedLeafValues(_rootNodes).ToList();

        if (selectedValues.Count == 0 || selectedValues.Count == CollectAllLeafValues(_rootNodes).Count())
        {
            // No filter or all selected — remove filter for this column
            var existing = FilterService.ColumnFilters
                .FirstOrDefault(f => f.ColumnModel is not null && f.ColumnModel.Equals(ColMetadata));
            if (existing is not null)
            {
                var empty = new GridFilterCellInfo { ColumnModel = ColMetadata, FilterCellValue = null, FilterCellMode = FilterCellModeConstants.InList };
                FilterService.AddOrRemoveFilter(empty);
            }
        }
        else
        {
            var filterInfo = new GridFilterCellInfo
            {
                ColumnModel = ColMetadata,
                FilterCellValue = selectedValues.Cast<object>().ToList(),
                FilterCellMode = FilterCellModeConstants.InList
            };
            FilterService.AddOrRemoveFilter(filterInfo);
        }

        await OnClose.InvokeAsync();
    }

    private async Task OnCancel()
    {
        await OnClose.InvokeAsync();
    }

    private static IEnumerable<object?> CollectCheckedLeafValues(IEnumerable<FilterMenuTreeNode> nodes)
    {
        foreach (var node in nodes)
        {
            if (node.IsLeaf)
            {
                if (node.IsChecked)
                    yield return node.RawValue;
            }
            else
            {
                foreach (var child in CollectCheckedLeafValues(node.Children))
                    yield return child;
            }
        }
    }

    private static IEnumerable<object?> CollectAllLeafValues(IEnumerable<FilterMenuTreeNode> nodes)
    {
        foreach (var node in nodes)
        {
            if (node.IsLeaf)
                yield return node.RawValue;
            else
                foreach (var child in CollectAllLeafValues(node.Children))
                    yield return child;
        }
    }

    public async ValueTask DisposeAsync()
    {
        FilterService.FilterCleared -= OnFilterCleared;
        if (_handlersRegistered)
            await JsService.RemoveFilterMenuDismissHandlers();
        _dotNetRef?.Dispose();
    }
}
