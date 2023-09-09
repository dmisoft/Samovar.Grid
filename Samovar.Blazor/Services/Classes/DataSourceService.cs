using Samovar.Blazor.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Samovar.Blazor
{
    public class DataSourceService<T>
        : IDataSourceService<T>
    {
        private readonly IFilterService _filterService;
        private readonly ISortingService _orderService;
        public ISubject<NavigationStrategyDataLoadingSettings> DataLoadingSettings { get; set; } = new ParameterSubject<NavigationStrategyDataLoadingSettings>(new NavigationStrategyDataLoadingSettings());

        public ISubject<IEnumerable<T>> Data { get; private set; } = new ParameterSubject<IEnumerable<T>>(new List<T>());

        Subscription3<IEnumerable<DataGridFilterCellInfo>, DataGridColumnOrderInfo, IEnumerable<T>, IQueryable<T>> DataQuerySubscription;
        public ISubject<IQueryable<T>> DataQuery { get; private set; } = new ParameterSubject<IQueryable<T>>();

            
        public DataSourceService(IFilterService filterService, ISortingService orderService)
        {
            _filterService = filterService;
            _orderService = orderService;
            
            DataQuerySubscription = new Subscription3<IEnumerable<DataGridFilterCellInfo>, DataGridColumnOrderInfo, IEnumerable<T>, IQueryable<T>>(_filterService.FilterInfo, _orderService.ColumnOrderInfo, Data, myfunc3);

            DataQuery = DataQuerySubscription.CreateMap();
        }

        private IQueryable<T> myfunc3(IEnumerable<DataGridFilterCellInfo> filterInfo, DataGridColumnOrderInfo orderInfo, IEnumerable<T> data)
        {
            if (data == null)
            {
                data = new List<T>();
                //Data = new ParameterSubject<IEnumerable<T>>(new List<T>());
            }

            IQueryable<T> query = data.AsQueryable();

            //apply filter
            if (filterInfo.Count() > 0)
                query = ApplyFilter(query, filterInfo);

            if (orderInfo != null && !orderInfo.Equals(DataGridColumnOrderInfo.Empty))
            {
                var pr = typeof(T).GetProperty(orderInfo.Field);

                query = orderInfo.Asc ? query.OrderBy(p => pr.GetValue(p)) : query.OrderByDescending(p => pr.GetValue(p));
            }

            return query;
        }
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
        private IQueryable<T> ApplyFilter(IQueryable<T> data, IEnumerable<DataGridFilterCellInfo> filterInfo)
        {
            Type t = typeof(T);
            ParameterExpression obj = Expression.Parameter(typeof(T));

            List<Expression> lambdaList = new List<Expression>();

            ConditionalExpression isNullExpression = null;

            foreach (var pair in filterInfo)
            {
                string field = pair.ColumnMetadata.Field.SubjectValue;
                DataGridFilterCellInfo filterCellInfo = pair;

                MemberExpression memberExp = Expression.Property(obj, field);

                switch (t.GetProperty(field).PropertyType)
                {
                    case var tt when tt == typeof(string):
                        ConstantExpression valueExp = Expression.Constant(filterCellInfo.FilterCellValue.ToString().ToLower());

                        MethodInfo IsNullOrEmptyMethod = typeof(string).GetMethod("IsNullOrEmpty", new[] { typeof(string) });
                        var nullValueSubstExpression = Expression.Assign(memberExp, Expression.Constant(""));
                        isNullExpression = Expression.IfThen(Expression.Call(IsNullOrEmptyMethod, memberExp), nullValueSubstExpression);

                        MethodInfo propertyToLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                        var callExp = Expression.Call(memberExp, propertyToLowerMethod);

                        switch (pair.FilterCellMode)
                        {
                            case 0: //*A*
                                MethodInfo containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                                var containsMethodExp = Expression.Call(callExp, containsMethod, valueExp);
                                lambdaList.Add(containsMethodExp);
                                break;
                            case 1: //=
                                lambdaList.Add(Expression.Equal(callExp, valueExp));
                                break;
                            case 2: //A*
                                MethodInfo startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
                                var startsWithMethodExp = Expression.Call(callExp, startsWithMethod, valueExp);
                                lambdaList.Add(startsWithMethodExp);
                                break;
                            case 3: //*A
                                MethodInfo endsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
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
                        //TODO
                        break;
                }
            }

            Expression retLambda = null;

            if (lambdaList.Count() == 1)
            {
                retLambda = lambdaList[0];
            }
            else if (lambdaList.Count() >= 2)
            {
                retLambda = Expression.AndAlso(lambdaList[0], lambdaList[1]);
                for (int i = 2; i < lambdaList.Count(); i++)
                {
                    retLambda = Expression.AndAlso(retLambda, lambdaList[i]);
                }
            }

            if (isNullExpression != null)
                retLambda = Expression.Block(new[] { isNullExpression, retLambda });

            var lambda = Expression.Lambda<Func<T, bool>>(retLambda, new ParameterExpression[] { obj });

            return data.Where(lambda);
        }
    }
}
