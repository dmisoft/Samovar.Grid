using Samovar.Grid.Filter;
using System.Collections;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;

namespace Samovar.Grid;

public class DataSourceService<T>
    : IDataSourceService<T>
    , IObserver<GridFilterMode>
{
    private readonly IFilterService _filterService;
    private readonly ISortingService _orderService;
    private readonly ILayoutService _layoutService;
    public BehaviorSubject<IEnumerable<T>> Data { get; private set; } = new BehaviorSubject<IEnumerable<T>>(new List<T>());

    public BehaviorSubject<IQueryable<T>?> DataQuery { get; private set; } = new BehaviorSubject<IQueryable<T>?>(null);

    public BehaviorSubject<Func<T, bool>?> CustomFilter { get; } = new BehaviorSubject<Func<T, bool>?>(null);

    readonly List<Type> numericTypeList =
        [
            typeof(byte),
            typeof(sbyte),
            typeof(int),
            typeof(uint),
            typeof(short),
            typeof(ushort),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(DateTime),
            typeof(DateTime?),
            typeof(DateOnly),
            typeof(DateOnly?),
            typeof(byte?),
            typeof(sbyte?),
            typeof(int?),
            typeof(uint?),
            typeof(short?),
            typeof(ushort?),
            typeof(long?),
            typeof(ulong?),
            typeof(float?),
            typeof(double?),
            typeof(decimal?)
        ];

    // String filter MethodInfo lookups, resolved once instead of on every filter emission
    // (AttachFilter previously re-ran typeof(string).GetMethod(...) on every keystroke).
    private static readonly MethodInfo StringToLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;
    private static readonly MethodInfo StringContainsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;
    private static readonly MethodInfo StringStartsWithMethod = typeof(string).GetMethod(nameof(string.StartsWith), new[] { typeof(string) })!;
    private static readonly MethodInfo StringEndsWithMethod = typeof(string).GetMethod(nameof(string.EndsWith), new[] { typeof(string) })!;
    private static readonly MethodInfo StringIsNullOrEmptyMethod = typeof(string).GetMethod(nameof(string.IsNullOrEmpty), new[] { typeof(string) })!;
    private static readonly MethodInfo StringListContainsMethod = typeof(List<string>).GetMethod(nameof(List<string>.Contains), new[] { typeof(string) })!;

    public DataSourceService(
          IFilterService filterService
        , ISortingService orderService
        , IInitService initService
        , ILayoutService layoutService

        )
    {
        _filterService = filterService;
        _orderService = orderService;
        initService.IsInitialized.Subscribe(DataGridInitializerCallback);
        _layoutService = layoutService;
    }
    IDisposable? observableStandardFilter = null;
    IDisposable? observableCustomFilter = null;

    private void DataGridInitializerCallback(bool obj)
    {
        _layoutService.FilterMode.DistinctUntilChanged().Subscribe(this);


    }

    private void ApplyStandardFilterAndSort(Tuple<IEnumerable<GridFilterCellInfo>, ColumnOrderInfo, IEnumerable<T>> tuple)
    {
        if (tuple.Item3 == null)
            return;

        IQueryable<T> query = tuple.Item3.AsQueryable();

        if (tuple.Item1.Any())
            query = AttachFilter(query, tuple.Item1);

        if (tuple.Item2 != null && !tuple.Item2.Equals(ColumnOrderInfo.Empty))
            query = SortKeySelectorFactory<T>.ApplyOrdering(query, tuple.Item2.Field, tuple.Item2.Asc);

        DataQuery.OnNext(Materialize(query));
    }

    private void ApplyCustomFilterAndSort(Tuple<Func<T, bool>?, ColumnOrderInfo, IEnumerable<T>> tuple)
    {
        if (tuple.Item3 == null)
            return;

        IQueryable<T> query = tuple.Item3.AsQueryable();


        if (tuple.Item1 is not null)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var body = Expression.Invoke(Expression.Constant(tuple.Item1), parameter);
            var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
            query = query.Where(lambda);
        }

        if (tuple.Item2 != null && !tuple.Item2.Equals(ColumnOrderInfo.Empty))
            query = SortKeySelectorFactory<T>.ApplyOrdering(query, tuple.Item2.Field, tuple.Item2.Asc);

        DataQuery.OnNext(Materialize(query));
    }

    /// <summary>
    /// Evaluates the filtered + sorted query once into an in-memory buffer so that downstream
    /// paging (Skip/Take in <see cref="RepositoryService{T}"/>) and repeated navigation changes
    /// do not re-run the filter/sort passes on every emission. Preserves <c>IQueryable&lt;T&gt;</c>
    /// so the rest of the pipeline is unchanged.
    /// </summary>
    private static IQueryable<T> Materialize(IQueryable<T> query) => query.ToArray().AsQueryable();

    private IQueryable<T> AttachFilter(IQueryable<T> data, IEnumerable<GridFilterCellInfo> filterInfo)
    {
        Type t = typeof(T);
        ParameterExpression obj = Expression.Parameter(typeof(T));

        List<Expression> lambdaList = [];

        ConditionalExpression? isNullExpression = null;

        foreach (var pair in filterInfo)
        {
            string field = pair.ColumnModel!.Field.Value;
            GridFilterCellInfo filterCellInfo = pair;

            MemberExpression memberExp = Expression.Property(obj, field);

            PropertyInfo? prop = t.GetProperty(field);
            if (prop is null)
                continue;

            switch (prop.PropertyType)
            {
                case var tt when tt == typeof(string):
                    if (pair.FilterCellMode == FilterCellModeConstants.InList)
                    {
                        var selectedStrings = ((IEnumerable<object>)filterCellInfo.FilterCellValue!)
                            .Select(v => v?.ToString()?.ToLower() ?? "").ToList();
                        var listConst = Expression.Constant(selectedStrings, typeof(List<string>));
                        var lowerMemberExp = Expression.Call(memberExp, StringToLowerMethod);
                        lambdaList.Add(Expression.Call(listConst, StringListContainsMethod, lowerMemberExp));
                        break;
                    }

                    var filterCellValue = filterCellInfo.FilterCellValue?.ToString() ?? "";
                    filterCellValue = filterCellValue.ToLower();
                    ConstantExpression valueExp = Expression.Constant(filterCellValue);

                    var nullValueSubstExpression = Expression.Assign(memberExp, Expression.Constant(""));
                    isNullExpression = Expression.IfThen(Expression.Call(StringIsNullOrEmptyMethod, memberExp), nullValueSubstExpression);

                    var callExp = Expression.Call(memberExp, StringToLowerMethod);

                    switch (pair.FilterCellMode)
                    {
                        case 0: //*A*
                            lambdaList.Add(Expression.Call(callExp, StringContainsMethod, valueExp));
                            break;
                        case 1: //=
                            lambdaList.Add(Expression.Equal(callExp, valueExp));
                            break;
                        case 2: //A*
                            lambdaList.Add(Expression.Call(callExp, StringStartsWithMethod, valueExp));
                            break;
                        case 3: //*A
                            lambdaList.Add(Expression.Call(callExp, StringEndsWithMethod, valueExp));
                            break;
                    }
                    break;
                case var tt when tt == typeof(char):
                    ConstantExpression charValueExp = Expression.Constant(filterCellInfo.FilterCellValue);
                    lambdaList.Add(Expression.Equal(memberExp, charValueExp));
                    break;
                case var tt when tt == typeof(bool):
                    ConstantExpression boolValueExp = Expression.Constant(filterCellInfo.FilterCellValue);
                    lambdaList.Add(Expression.Equal(memberExp, boolValueExp));
                    break;
                case var tt when numericTypeList.Contains(tt):
                    if (pair.FilterCellMode == FilterCellModeConstants.InList)
                    {
                        var rawValues = ((IEnumerable<object>)filterCellInfo.FilterCellValue!).ToList();
                        var typedValues = rawValues
                            .Select(v => v is null ? null : Convert.ChangeType(v, Nullable.GetUnderlyingType(tt) ?? tt))
                            .ToList();
                        var typedListType = typeof(List<>).MakeGenericType(tt);
                        var typedList = Activator.CreateInstance(typedListType)!;
                        var addMethod = typedListType.GetMethod("Add")!;
                        foreach (var v in typedValues)
                            addMethod.Invoke(typedList, new[] { v });
                        var numericListConst = Expression.Constant(typedList, typedListType);
                        MethodInfo numericContainsMethod = typedListType.GetMethod("Contains", new[] { tt })!;
                        lambdaList.Add(Expression.Call(numericListConst, numericContainsMethod, memberExp));
                        break;
                    }
                    // DateTime: compare date part only, time component is ignored
                    if (tt == typeof(DateTime) || tt == typeof(DateTime?))
                    {
                        if (filterCellInfo.FilterCellValue == null) break;
                        var rawDt = filterCellInfo.FilterCellValue is DateTime d
                            ? d
                            : ((DateTime?)filterCellInfo.FilterCellValue)!.Value;
                        var filterDateConst = Expression.Constant(rawDt.Date, typeof(DateTime));
                        if (tt == typeof(DateTime?))
                        {
                            var hasValue = Expression.Property(memberExp, "HasValue");
                            var memberValue = Expression.Property(memberExp, "Value");
                            var memberDate = Expression.Property(memberValue, nameof(DateTime.Date));
                            Expression? cmp = pair.FilterCellMode switch
                            {
                                0 => Expression.Equal(memberDate, filterDateConst),
                                1 => Expression.GreaterThan(memberDate, filterDateConst),
                                2 => Expression.GreaterThanOrEqual(memberDate, filterDateConst),
                                3 => Expression.LessThan(memberDate, filterDateConst),
                                4 => Expression.LessThanOrEqual(memberDate, filterDateConst),
                                _ => null
                            };
                            if (cmp != null)
                                lambdaList.Add(Expression.AndAlso(hasValue, cmp));
                        }
                        else
                        {
                            var memberDate = Expression.Property(memberExp, nameof(DateTime.Date));
                            Expression? cmp = pair.FilterCellMode switch
                            {
                                0 => Expression.Equal(memberDate, filterDateConst),
                                1 => Expression.GreaterThan(memberDate, filterDateConst),
                                2 => Expression.GreaterThanOrEqual(memberDate, filterDateConst),
                                3 => Expression.LessThan(memberDate, filterDateConst),
                                4 => Expression.LessThanOrEqual(memberDate, filterDateConst),
                                _ => null
                            };
                            if (cmp != null)
                                lambdaList.Add(cmp);
                        }
                        break;
                    }
                    Expression numericValueExp = Expression.Convert(Expression.Constant(filterCellInfo.FilterCellValue), tt);
                    switch (pair.FilterCellMode)
                    {
                        case 0:// =
                            lambdaList.Add(Expression.Equal(memberExp, numericValueExp));
                            break;
                        case 1:// >
                            lambdaList.Add(Expression.GreaterThan(memberExp, numericValueExp));
                            break;
                        case 2:// >=
                            lambdaList.Add(Expression.GreaterThanOrEqual(memberExp, numericValueExp));
                            break;
                        case 3:// <
                            lambdaList.Add(Expression.LessThan(memberExp, numericValueExp));
                            break;
                        case 4:// <=
                            lambdaList.Add(Expression.LessThanOrEqual(memberExp, numericValueExp));
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        Expression? retLambda = null;

        if (lambdaList.Count == 1)
        {
            retLambda = lambdaList[0];
        }
        else if (lambdaList.Count >= 2)
        {
            retLambda = Expression.AndAlso(lambdaList[0], lambdaList[1]);
            for (int i = 2; i < lambdaList.Count; i++)
            {
                retLambda = Expression.AndAlso(retLambda, lambdaList[i]);
            }
        }

        if (isNullExpression is not null && retLambda is not null)
            retLambda = Expression.Block(isNullExpression, retLambda);


        if (retLambda is not null)
            retLambda = Expression.Lambda<Func<T, bool>>(retLambda, obj);

        if (retLambda is not null)
            return data.Where((Expression<Func<T, bool>>)retLambda);
        else
            return data;
    }

    public IEnumerable<object?> GetDistinctColumnValues(string field)
    {
        var prop = typeof(T).GetProperty(field);
        if (prop is null) return Enumerable.Empty<object?>();
        return Data.Value
            .Select(item => prop.GetValue(item))
            .Distinct()
            .OrderBy(v => v)
            .ToList();
    }

    public void OnCompleted()
    {
        observableStandardFilter?.Dispose();
        observableCustomFilter?.Dispose();
    }

    public void OnError(Exception error)
    {
        // BehaviorSubject<GridFilterMode> does not emit errors in normal operation
    }

    public void OnNext(GridFilterMode value)
    {
        CustomFilter.OnNext(null);
        _filterService.ClearFilter();

        observableStandardFilter?.Dispose();
        observableCustomFilter?.Dispose();

        if (value == GridFilterMode.Custom)
        {
            observableCustomFilter = Observable.CombineLatest(
                CustomFilter,
                 _orderService.ColumnOrderInfo,
                 Data,
                 (filterInfo, columnOrderInfo, data) => Tuple.Create(filterInfo, columnOrderInfo, data))
                 .DistinctUntilChanged()
                 .Subscribe(ApplyCustomFilterAndSort);
        }
        else
        {
            // FilterRow and FilterMenu both use the FilterService reactive pipeline
            observableStandardFilter = Observable.CombineLatest(
                _filterService.FilterInfo,
                _orderService.ColumnOrderInfo,
                Data,
                (filterInfo, columnOrderInfo, data) => Tuple.Create(filterInfo, columnOrderInfo, data))
                .DistinctUntilChanged()
                .Subscribe(ApplyStandardFilterAndSort);
        }
    }
}
