using Samovar.Blazor.Filter;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;

namespace Samovar.Blazor
{
    public class DataSourceService<T>
        : IDataSourceService<T>
    {
        private readonly IFilterService _filterService;
        private readonly ISortingService _orderService;

        public BehaviorSubject<IEnumerable<T>> Data { get; private set; } = new BehaviorSubject<IEnumerable<T>>(new List<T>());

        public BehaviorSubject<IQueryable<T>> DataQuery { get; private set; } = new BehaviorSubject<IQueryable<T>>(null);

        List<Type> numericTypeList = new List<Type>
                {
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
                    typeof(decimal?),
                    typeof(DateTime?)
            };

        public DataSourceService(
              IFilterService filterService
            , ISortingService orderService
            , IInitService initService
            )
        {
            _filterService = filterService;
            _orderService = orderService;
            initService.IsInitialized.Subscribe(DataGridInitializerCallback);
        }

        private void DataGridInitializerCallback(bool obj)
        {
            //combine
            var rr = Observable.CombineLatest(
                _filterService.FilterInfo,
                _orderService.ColumnOrderInfo,
                Data,
                (filterInfo, columnOrderInfo, data) => Tuple.Create(filterInfo, columnOrderInfo, data)
             ).DistinctUntilChanged();
            rr.Subscribe(myfunc33);
        }

        private void myfunc33(Tuple<IEnumerable<DataGridFilterCellInfo>, DataGridColumnOrderInfo, IEnumerable<T>> tuple)
        {
            if (tuple.Item3 == null)
            {
                return;
            }

            IQueryable<T> query = tuple.Item3.AsQueryable();

            if (tuple.Item1.Any())
                query = AttachFilter(query, tuple.Item1);

            if (tuple.Item2 != null && !tuple.Item2.Equals(DataGridColumnOrderInfo.Empty))
            {
                var pr = typeof(T).GetProperty(tuple.Item2.Field);

                query = tuple.Item2.Asc ? query.OrderBy(p => pr.GetValue(p)) : query.OrderByDescending(p => pr.GetValue(p));
            }
            DataQuery.OnNext(query);
        }

        private IQueryable<T> AttachFilter(IQueryable<T> data, IEnumerable<DataGridFilterCellInfo> filterInfo)
        {
            Type t = typeof(T);
            ParameterExpression obj = Expression.Parameter(typeof(T));

            List<Expression> lambdaList = new List<Expression>();

            ConditionalExpression isNullExpression = null;

            foreach (var pair in filterInfo)
            {
                string field = pair.ColumnMetadata.Field.Value;
                DataGridFilterCellInfo filterCellInfo = pair;

                MemberExpression memberExp = Expression.Property(obj, field);

                PropertyInfo? prop = t.GetProperty(field);
                if (prop is null)
                    continue;

                switch (prop.PropertyType)
                {
                    case var tt when tt == typeof(string):
                        var filterCellValue = filterCellInfo.FilterCellValue.ToString().ToLower();
                        ConstantExpression valueExp = Expression.Constant(filterCellValue);

                        MethodInfo? IsNullOrEmptyMethod = typeof(string).GetMethod("IsNullOrEmpty", new[] { typeof(string) });
                        if (IsNullOrEmptyMethod is null)
                            break;

                        var nullValueSubstExpression = Expression.Assign(memberExp, Expression.Constant(""));
                        isNullExpression = Expression.IfThen(Expression.Call(IsNullOrEmptyMethod, memberExp), nullValueSubstExpression);

                        MethodInfo? propertyToLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                        if (propertyToLowerMethod is null)
                            break;
                        var callExp = Expression.Call(memberExp, propertyToLowerMethod);

                        switch (pair.FilterCellMode)
                        {
                            case 0: //*A*
                                MethodInfo? containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                                if (containsMethod is null)
                                    break;
                                var containsMethodExp = Expression.Call(callExp, containsMethod, valueExp);
                                lambdaList.Add(containsMethodExp);
                                break;
                            case 1: //=
                                lambdaList.Add(Expression.Equal(callExp, valueExp));
                                break;
                            case 2: //A*
                                MethodInfo? startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
                                if (startsWithMethod is null)
                                    break;
                                var startsWithMethodExp = Expression.Call(callExp, startsWithMethod, valueExp);
                                lambdaList.Add(startsWithMethodExp);
                                break;
                            case 3: //*A
                                MethodInfo? endsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
                                if (endsWithMethod is null)
                                    break;
                                var endsWithMethodExp = Expression.Call(callExp, endsWithMethod, valueExp);
                                lambdaList.Add(endsWithMethodExp);
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

            if (isNullExpression != null)
                retLambda = Expression.Block(new[] { isNullExpression, retLambda });

            var lambda = Expression.Lambda<Func<T, bool>>(retLambda, new ParameterExpression[] { obj });

            return data.Where(lambda);

            //if (isNullExpression is not null && retLambda is not null) {
            //    retLambda = Expression.Block(isNullExpression, retLambda);
            //    var lambda = Expression.Lambda<Func<T, bool>>(retLambda, obj );
            //    return data.Where(lambda);
            //}

            //return data;
        }
    }
}
