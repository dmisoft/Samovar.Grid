using Samovar.Blazor.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;

namespace Samovar.Blazor
{
    public class DataSourceService<T>
        : IDataSourceService<T>
    {
        private readonly IInitService _initService;
        private readonly IFilterService _filterService;
        private readonly ISortingService _orderService;
        private readonly INavigationService _navigationService;

        public BehaviorSubject<NavigationStrategyDataLoadingSettings> DataLoadingSettings { get; set; } = new BehaviorSubject<NavigationStrategyDataLoadingSettings>(new NavigationStrategyDataLoadingSettings());

        public BehaviorSubject<IEnumerable<T>> Data { get; private set; } = new BehaviorSubject<IEnumerable<T>>(new List<T>());

        public IObservable<IQueryable<T>> DataQuery { get; private set; }

        public DataSourceService(
              IFilterService filterService
            , ISortingService orderService
            , IInitService initService
            //, INavigationService navigationService

            )
        {
            _filterService = filterService;
            _orderService = orderService;
            _initService = initService;
            //_navigationService = navigationService;
            _initService.IsInitialized.Subscribe(DataGridInitializerCallback);
        }

        private void DataGridInitializerCallback(bool obj)
        {
            //combine
            DataQuery = Observable.CombineLatest(
                _filterService.FilterInfo,
                _orderService.ColumnOrderInfo,
                Data,
                myfunc3
             );
        }


        //private void dataObserver(IEnumerable<T> data)
        //{
        //    if (data == null)
        //    {
        //        data = new List<T>();
        //        //Data = new ParameterSubject<IEnumerable<T>>(new List<T>());
        //    }

        //    IQueryable<T> query = data.AsQueryable();

        //    //apply filter
        //    if (_filterService.FilterInfo.Value.Count() > 0)
        //        query = ApplyFilter(query, _filterService.FilterInfo.Value);

        //    if (_orderService.ColumnOrderInfo != null && !_orderService.ColumnOrderInfo.Equals(DataGridColumnOrderInfo.Empty))
        //    {
        //        //var pr = typeof(T).GetProperty(_orderService.ColumnOrderInfo.Value.Field);
        //        //query = _orderService.ColumnOrderInfo.Value.Asc ? query.OrderBy(p => pr.GetValue(p)) : query.OrderByDescending(p => pr.GetValue(p));
        //    }
        //    DataQuery.OnNext(query);
        //    //TODO refactoring 10/2023
        //    //throw new NotImplementedException();
        //}



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
                query = AttachFilter(query, filterInfo);

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
