using Samovar.Grid.Formulas;
using System.Collections;
using System.Globalization;
using System.Reactive.Subjects;
using System.Reflection;

namespace Samovar.Grid;

public class SummaryFooterService<T>
    : ISummaryFooterService, IDisposable
{
    private readonly List<SummaryDescriptor> _summaries = new();
    private readonly Dictionary<string, PropertyInfo?> _propertyCache = new(StringComparer.Ordinal);
    private readonly IDataSourceService<T> _dataSourceService;
    private readonly IColumnService _columnService;
    private readonly IRepositoryService<T> _repositoryService;
    private readonly IDisposable _dataQuerySubscription;

    public IReadOnlyList<SummaryDescriptor> Summaries => _summaries;

    public bool HasAny => _summaries.Count > 0;

    public BehaviorSubject<IReadOnlyDictionary<SummaryDescriptor, object?>> Values { get; }
        = new BehaviorSubject<IReadOnlyDictionary<SummaryDescriptor, object?>>(
            new Dictionary<SummaryDescriptor, object?>());

    public SummaryFooterService(IDataSourceService<T> dataSourceService, IColumnService columnService, IRepositoryService<T> repositoryService)
    {
        _dataSourceService = dataSourceService;
        _columnService = columnService;
        _repositoryService = repositoryService;
        _dataQuerySubscription = _dataSourceService.DataQuery.Subscribe(_ => Recalculate());
    }

    public IReadOnlyList<SummaryDescriptor> GetForField(string field)
    {
        if (string.IsNullOrEmpty(field))
            return Array.Empty<SummaryDescriptor>();

        return _summaries
            .Where(s => string.Equals(s.Field, field, StringComparison.Ordinal)
                && s.Aggregate != GridSummaryAggregate.None)
            .ToList();
    }

    public void RegisterSummary(SummaryDescriptor descriptor)
    {
        _summaries.Add(descriptor);
        Recalculate();
    }

    public void Recalculate()
    {
        if (_summaries.Count == 0)
        {
            Values.OnNext(new Dictionary<SummaryDescriptor, object?>());
            return;
        }

        IEnumerable? source = _dataSourceService.DataQuery.Value
            ?? (IEnumerable?)_dataSourceService.Data.Value;

        var materialized = MaterializeSource(source);
        var result = new Dictionary<SummaryDescriptor, object?>();

        foreach (var descriptor in _summaries)
        {
            if (descriptor.Aggregate == GridSummaryAggregate.None)
                continue;

            if (TryResolveExpressionGetter(descriptor.Field, out var getter))
            {
                result[descriptor] = ComputeAggregateFromGetter(descriptor.Aggregate, materialized, getter);
                continue;
            }

            var prop = ResolveProperty(descriptor.Field);
            result[descriptor] = ComputeAggregate(descriptor.Aggregate, materialized, prop);
        }

        Values.OnNext(result);
    }

    private static IReadOnlyList<object?> MaterializeSource(IEnumerable? source)
    {
        if (source is null)
            return Array.Empty<object?>();

        var list = new List<object?>();
        foreach (var item in source)
            list.Add(item);
        return list;
    }

    private bool TryResolveExpressionGetter(string field, out Func<T, decimal?> getter)
    {
        foreach (var column in _columnService.DataColumnModels)
        {
            if (column is not IExpressionColumnModel expr) continue;
            if (!string.Equals(expr.Field.Value, field, StringComparison.Ordinal)) continue;

            var typed = (Func<T, decimal?>?)expr.CompiledGetter;
            if (typed is null)
            {
                typed = expr.Ast is null
                    ? _ => null
                    : FormulaCompiler.Compile<T>(expr.Ast, _repositoryService.PropInfo);
                expr.CompiledGetter = typed;
            }
            getter = typed;
            return true;
        }
        getter = null!;
        return false;
    }

    private static object? ComputeAggregateFromGetter(GridSummaryAggregate aggregate, IReadOnlyList<object?> rows, Func<T, decimal?> getter)
    {
        if (rows.Count == 0 && aggregate != GridSummaryAggregate.Count)
            return null;

        switch (aggregate)
        {
            case GridSummaryAggregate.Count:
            {
                int count = 0;
                for (int i = 0; i < rows.Count; i++)
                {
                    if (rows[i] is not T row) continue;
                    if (getter(row).HasValue) count++;
                }
                return count;
            }
            case GridSummaryAggregate.Sum:
            {
                decimal sum = 0m;
                for (int i = 0; i < rows.Count; i++)
                {
                    if (rows[i] is not T row) continue;
                    var v = getter(row);
                    if (v.HasValue) sum += v.Value;
                }
                return sum;
            }
            case GridSummaryAggregate.Average:
            {
                decimal sum = 0m;
                int n = 0;
                for (int i = 0; i < rows.Count; i++)
                {
                    if (rows[i] is not T row) continue;
                    var v = getter(row);
                    if (!v.HasValue) continue;
                    sum += v.Value;
                    n++;
                }
                return n == 0 ? null : sum / n;
            }
            case GridSummaryAggregate.Min:
            case GridSummaryAggregate.Max:
            {
                decimal? best = null;
                bool takeMin = aggregate == GridSummaryAggregate.Min;
                for (int i = 0; i < rows.Count; i++)
                {
                    if (rows[i] is not T row) continue;
                    var v = getter(row);
                    if (!v.HasValue) continue;
                    if (best is null || (takeMin ? v.Value < best.Value : v.Value > best.Value))
                        best = v.Value;
                }
                return best;
            }
            default:
                return null;
        }
    }

    private PropertyInfo? ResolveProperty(string field)
    {
        if (_propertyCache.TryGetValue(field, out var cached))
            return cached;

        var prop = typeof(T).GetProperty(field, BindingFlags.Public | BindingFlags.Instance);
        _propertyCache[field] = prop;
        return prop;
    }

    private static object? ComputeAggregate(GridSummaryAggregate aggregate, IReadOnlyList<object?> rows, PropertyInfo? prop)
    {
        if (aggregate == GridSummaryAggregate.Count)
        {
            if (prop is null)
                return rows.Count;

            int count = 0;
            for (int i = 0; i < rows.Count; i++)
            {
                if (rows[i] is null) continue;
                if (prop.GetValue(rows[i]) is not null)
                    count++;
            }
            return count;
        }

        if (prop is null || rows.Count == 0)
            return null;

        switch (aggregate)
        {
            case GridSummaryAggregate.Sum:
                return TrySumDecimal(rows, prop, out var sum) ? sum : (object?)null;

            case GridSummaryAggregate.Average:
                if (!TrySumDecimal(rows, prop, out var avgSum, out var nonNullCount))
                    return null;
                return nonNullCount == 0 ? null : avgSum / nonNullCount;

            case GridSummaryAggregate.Min:
                return MinMax(rows, prop, takeMin: true);

            case GridSummaryAggregate.Max:
                return MinMax(rows, prop, takeMin: false);

            default:
                return null;
        }
    }

    private static bool TrySumDecimal(IReadOnlyList<object?> rows, PropertyInfo prop, out decimal sum)
        => TrySumDecimal(rows, prop, out sum, out _);

    private static bool TrySumDecimal(IReadOnlyList<object?> rows, PropertyInfo prop, out decimal sum, out int nonNullCount)
    {
        sum = 0m;
        nonNullCount = 0;
        for (int i = 0; i < rows.Count; i++)
        {
            if (rows[i] is null) continue;
            var value = prop.GetValue(rows[i]);
            if (value is null) continue;
            try
            {
                sum += Convert.ToDecimal(value, CultureInfo.InvariantCulture);
                nonNullCount++;
            }
            catch (InvalidCastException)
            {
                return false;
            }
            catch (FormatException)
            {
                return false;
            }
            catch (OverflowException)
            {
                return false;
            }
        }
        return true;
    }

    private static object? MinMax(IReadOnlyList<object?> rows, PropertyInfo prop, bool takeMin)
    {
        object? best = null;
        for (int i = 0; i < rows.Count; i++)
        {
            if (rows[i] is null) continue;
            var value = prop.GetValue(rows[i]);
            if (value is null) continue;

            if (best is null)
            {
                best = value;
                continue;
            }

            int cmp;
            try
            {
                cmp = Comparer<object>.Default.Compare(value, best);
            }
            catch (ArgumentException)
            {
                return null;
            }

            if (takeMin ? cmp < 0 : cmp > 0)
                best = value;
        }
        return best;
    }

    public void Dispose()
    {
        _dataQuerySubscription.Dispose();
    }
}
