using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Samovar.Grid;

/// <summary>
/// Builds and caches strongly-typed ordering appliers for a given (T, field) pair.
///
/// The previous implementation sorted via <c>query.OrderBy(p =&gt; propertyInfo.GetValue(p))</c>,
/// which performs a reflection call and boxes the value for every row on every sort. This factory
/// compiles a <see cref="LambdaExpression"/> property accessor once per field and invokes the
/// generic <see cref="Queryable.OrderBy{TSource,TKey}(IQueryable{TSource}, Expression{Func{TSource,TKey}})"/>
/// so ordering uses the property's real type (e.g. numeric / DateTime) rather than boxed objects.
/// </summary>
internal static class SortKeySelectorFactory<T>
{
    // Cache keyed by field name. Each entry knows how to apply ascending/descending ordering
    // for that field, with the property-access lambda already built.
    private static readonly ConcurrentDictionary<string, Func<IQueryable<T>, bool, IQueryable<T>>> _cache = new();

    private static readonly MethodInfo OrderByMethod =
        typeof(Queryable).GetMethods()
            .First(m => m.Name == nameof(Queryable.OrderBy) && m.GetParameters().Length == 2);

    private static readonly MethodInfo OrderByDescendingMethod =
        typeof(Queryable).GetMethods()
            .First(m => m.Name == nameof(Queryable.OrderByDescending) && m.GetParameters().Length == 2);

    /// <summary>
    /// Applies ordering on <paramref name="query"/> by <paramref name="field"/>.
    /// Returns the query unchanged if the field does not exist on <typeparamref name="T"/>.
    /// </summary>
    public static IQueryable<T> ApplyOrdering(IQueryable<T> query, string field, bool ascending)
    {
        var applier = _cache.GetOrAdd(field, BuildApplier);
        return applier(query, ascending);
    }

    private static Func<IQueryable<T>, bool, IQueryable<T>> BuildApplier(string field)
    {
        PropertyInfo? prop = typeof(T).GetProperty(field);
        if (prop is null)
            return static (query, _) => query; // unknown field: leave ordering unchanged

        // p => p.Field  (typed as Expression<Func<T, TKey>>)
        ParameterExpression param = Expression.Parameter(typeof(T), "p");
        MemberExpression propertyAccess = Expression.Property(param, prop);
        LambdaExpression keySelector = Expression.Lambda(propertyAccess, param);

        MethodInfo orderBy = OrderByMethod.MakeGenericMethod(typeof(T), prop.PropertyType);
        MethodInfo orderByDescending = OrderByDescendingMethod.MakeGenericMethod(typeof(T), prop.PropertyType);

        return (query, ascending) =>
        {
            MethodInfo method = ascending ? orderBy : orderByDescending;
            return (IQueryable<T>)method.Invoke(null, new object[] { query, keySelector })!;
        };
    }
}
