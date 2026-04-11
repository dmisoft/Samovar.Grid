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

    [SmInject]
    public required ILayoutService LayoutService { get; set; }

    private string _btnSizeClass = "";
    private string _inputGroupSizeClass = "";
    private string _formControlSizeClass = "";
    private string _listItemSizeClass = "";

    private ElementReference _panelRef;
    private DotNetObjectReference<GridColumnFilterMenuPanel<TItem>>? _dotNetRef;
    private bool _handlersRegistered = false;

    private List<FilterMenuTreeNode> _rootNodes = new();
    private string _searchText = "";
    private bool _isDateColumn;

    private static readonly HashSet<Type> DateTypes = new()
    {
        typeof(DateTime), typeof(DateTime?),
        typeof(DateOnly), typeof(DateOnly?)
    };

    private const int NumericGroupingThreshold = 20;

    private static readonly HashSet<Type> IntegralTypes = new()
    {
        typeof(byte),    typeof(byte?),
        typeof(sbyte),   typeof(sbyte?),
        typeof(short),   typeof(short?),
        typeof(ushort),  typeof(ushort?),
        typeof(int),     typeof(int?),
        typeof(uint),    typeof(uint?),
        typeof(long),    typeof(long?),
        typeof(ulong),   typeof(ulong?),
    };

    private static readonly HashSet<Type> NumericGroupableTypes = new()
    {
        typeof(byte),    typeof(byte?),
        typeof(sbyte),   typeof(sbyte?),
        typeof(short),   typeof(short?),
        typeof(ushort),  typeof(ushort?),
        typeof(int),     typeof(int?),
        typeof(uint),    typeof(uint?),
        typeof(long),    typeof(long?),
        typeof(ulong),   typeof(ulong?),
        typeof(float),   typeof(float?),
        typeof(double),  typeof(double?),
        typeof(decimal), typeof(decimal?),
    };

    protected override Task OnInitializedAsync()
    {
        var field = ColMetadata.Field.Value;
        var rawValues = DataSourceService.GetDistinctColumnValues(field);

        PropertyInfo? propInfo = null;
        RepositoryService.PropInfo.TryGetValue(field, out propInfo);
        var propType = propInfo?.PropertyType ?? typeof(string);

        _isDateColumn = DateTypes.Contains(propType);

        if (_isDateColumn)
            _rootNodes = BuildDateHierarchy(rawValues, propType);
        else if (NumericGroupableTypes.Contains(propType))
            _rootNodes = BuildNumericHierarchy(rawValues, propType);
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

        LayoutService.SizeMode.Subscribe(mode =>
        {
            _btnSizeClass         = mode switch { GridSizeMode.Small => "btn-sm small", GridSizeMode.Large => "btn-lg", _ => "" };
            _inputGroupSizeClass  = mode switch { GridSizeMode.Small => "input-group-sm", GridSizeMode.Large => "input-group-lg", _ => "" };
            _formControlSizeClass = mode switch { GridSizeMode.Small => "form-control-sm", GridSizeMode.Large => "form-control-lg", _ => "" };
            _listItemSizeClass    = mode switch { GridSizeMode.Small => "small", GridSizeMode.Large => "fs-5", _ => "" };
            StateHasChanged();
        });

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
                            IsExpanded = false,
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
                            IsExpanded = false,
                            Children = mg.Select(d => new FilterMenuTreeNode
                            {
                                Label = d.Day.ToString(),
                                RawValue = d
                            }).ToList()
                        }).ToList()
                }).ToList();
        }
    }

    private static decimal ComputeBucketSize(decimal min, decimal max, int targetGroups = 10)
    {
        decimal range = max - min;
        if (range <= 0) return 1m;

        decimal rawSize = range / targetGroups;
        double log = Math.Floor(Math.Log10((double)rawSize));
        decimal magnitude = (decimal)Math.Pow(10, log);

        foreach (var m in new[] { 1m, 2m, 5m, 10m })
        {
            decimal candidate = magnitude * m;
            if (Math.Ceiling(range / candidate) <= 15m)
                return candidate;
        }
        return magnitude * 10m;
    }

    private static string FormatBucketBound(decimal value, Type propType) =>
        IntegralTypes.Contains(propType) ? ((long)value).ToString() : value.ToString("G");

    private static List<FilterMenuTreeNode> BuildNumericHierarchy(IEnumerable<object?> values, Type propType)
    {
        var materialised = values.ToList();
        var nullNodes = materialised
            .Where(v => v is null)
            .Select(_ => new FilterMenuTreeNode { Label = "(blank)", RawValue = null })
            .ToList();

        List<(object raw, decimal asDecimal)> numericValues;
        try
        {
            numericValues = materialised
                .Where(v => v is not null)
                .Select(v => (raw: v!, asDecimal: Convert.ToDecimal(v)))
                .OrderBy(x => x.asDecimal)
                .ToList();
        }
        catch
        {
            return BuildFlatList(materialised);
        }

        if (numericValues.Count == 0)
            return nullNodes;

        if (numericValues.Count <= NumericGroupingThreshold)
        {
            var flat = numericValues
                .Select(x => new FilterMenuTreeNode { Label = x.raw.ToString() ?? "", RawValue = x.raw })
                .ToList();
            flat.AddRange(nullNodes);
            return flat;
        }

        decimal minVal = numericValues[0].asDecimal;
        decimal maxVal = numericValues[^1].asDecimal;
        decimal bucketSize = ComputeBucketSize(minVal, maxVal);
        decimal bucketFloor = Math.Floor(minVal / bucketSize) * bucketSize;

        var buckets = new SortedDictionary<decimal, List<(object raw, decimal asDecimal)>>();
        foreach (var (raw, asDecimal) in numericValues)
        {
            decimal bucketStart = Math.Floor((asDecimal - bucketFloor) / bucketSize) * bucketSize + bucketFloor;
            if (!buckets.ContainsKey(bucketStart))
                buckets[bucketStart] = new();
            buckets[bucketStart].Add((raw, asDecimal));
        }

        var groupNodes = buckets.Select(kv =>
        {
            decimal bucketStart = kv.Key;
            decimal bucketEnd = IntegralTypes.Contains(propType)
                ? bucketStart + bucketSize - 1m
                : bucketStart + bucketSize;

            string groupLabel = $"{FormatBucketBound(bucketStart, propType)} - {FormatBucketBound(bucketEnd, propType)}";

            var children = kv.Value
                .Select(x => new FilterMenuTreeNode { Label = x.raw.ToString() ?? "", RawValue = x.raw })
                .ToList();

            if (children.Count == 1)
                return children[0];

            return new FilterMenuTreeNode { Label = groupLabel, IsExpanded = false, Children = children };
        }).ToList();

        groupNodes.AddRange(nullNodes);
        return groupNodes;
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
        if (node.Label.Contains(_searchText, StringComparison.OrdinalIgnoreCase))
            return true;
        if (!node.IsLeaf)
            return node.Children.Any(NodeMatchesSearch);
        return false;
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
