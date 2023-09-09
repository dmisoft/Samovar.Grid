using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Samovar.DataGrid
{
    internal class GridDataRepository<T>
        : IGridDataRepository<T>, IDisposable
    {
        public List<T> Data { get; private set; } = new List<T>();

        public event Func<List<T>, Task> OnNotifyDataLoaded;

        #region ctor
        public GridDataRepository(IEnumerable<T> data)
        {
            Data = data.ToList();
        }

        #endregion

        public void InsertItems(IList itemsToInsert, int newSartingIndex)
        {
            foreach (T item in itemsToInsert)
            {
                Data.Insert(newSartingIndex++, item);
            }
        }

        public void RemoveItems(IList itemsToRemove)
        {
            foreach (T item in itemsToRemove)
            {
                T kv = Data.SingleOrDefault(v => v.Equals(item));
                if (!kv.Equals(default(T))) Data.Remove(kv);
            }
        }

        public void RemoveAllItems()
        {
            Data.Clear();
        }

        public Task<PageableViewCollectionInfo<T>> GetPageData(Dictionary<string, FilterCellInfo> filterData, int pageNumber, int pageSize, string sortingColumn, bool? ascending, Func<T, bool> customFilter, GridFilterMode filterMode)
        {
            int skip = (pageNumber - 1) * pageSize;
            int take = pageSize;
            return Task.Run(() =>
            {
                (IEnumerable<T> Items, int FilteredDataCount) data = GetData(skip, take, sortingColumn, ascending, filterData, customFilter, filterMode);
                PageableViewCollectionInfo<T> retVal = new PageableViewCollectionInfo<T>();
                retVal.PageData = data.Items;
                retVal.FilteredDataCount = data.FilteredDataCount;
                retVal.PageDataCount = Math.Min(retVal.FilteredDataCount, pageSize);

                return retVal;
            });
        }
        public PageableViewCollectionInfo<T> GetPageData_V6(Dictionary<string, FilterCellInfo> filterData, int pageNumber, int pageSize, string sortingColumn, bool? ascending, Func<T, bool> customFilter, GridFilterMode filterMode)
        {
            int skip = (pageNumber - 1) * pageSize;
            int take = pageSize;
            (IEnumerable<T> Items, int FilteredDataCount) data = GetData(skip, take, sortingColumn, ascending, filterData, customFilter, filterMode);
            PageableViewCollectionInfo<T> retVal = new PageableViewCollectionInfo<T>();
            retVal.PageData = data.Items;
            retVal.FilteredDataCount = data.FilteredDataCount;
            retVal.PageDataCount = Math.Min(retVal.FilteredDataCount, pageSize);

            return retVal;
        }

        public (IEnumerable<T>, int) GetData(int skip, int take, string sortingColumn, bool? ascending, Dictionary<string, FilterCellInfo> filterData, Func<T, bool> customFilter, GridFilterMode filterMode)
        {
            IEnumerable<T> tempDataList = null;
            int filteredItemsCount = Data.Count();

            if (string.IsNullOrEmpty(sortingColumn))
            {
                switch (filterMode)
                {
                    case GridFilterMode.None:
                        tempDataList = Data.AsQueryable().Skip(skip).Take(take);
                        break;
                    case GridFilterMode.Custom:
                        if (customFilter != null)
                        {
                            var tempFilteredData = CustomFilter(customFilter);
                            filteredItemsCount = tempFilteredData.Count();
                            tempDataList = tempFilteredData.Skip(skip).Take(take);
                        }
                        else
                        {
                            tempDataList = Data.AsQueryable().Skip(skip).Take(take);
                        }
                        break;
                    case GridFilterMode.FilterRow:
                        if (filterData.Count() > 0)
                        {
                            var tempFilteredData = Filter(filterData);
                            filteredItemsCount = tempFilteredData.Count();
                            tempDataList = tempFilteredData.Skip(skip).Take(take);
                        }
                        else
                        {
                            tempDataList = Data.AsQueryable().Skip(skip).Take(take);
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                var pr = typeof(T).GetProperty(sortingColumn);
                switch (filterMode)
                {
                    case GridFilterMode.None:
                        if (ascending.HasValue)
                        {
                            if (ascending.Value)
                            {
                                tempDataList = Data.AsQueryable().OrderBy(pair => pr.GetValue(pair)).Skip(skip).Take(take);
                            }
                            else
                            {
                                tempDataList = Data.AsQueryable().OrderByDescending(pair => pr.GetValue(pair)).Skip(skip).Take(take);
                            }
                        }
                        else
                        {
                            tempDataList = Data.AsQueryable().Skip(skip).Take(take);
                        }
                        break;
                    case GridFilterMode.Custom:
                        if (customFilter != null)
                        {
                            var tempFilteredData = CustomFilter(customFilter);
                            filteredItemsCount = tempFilteredData.Count();
                            if (ascending.HasValue)
                            {
                                if (ascending.Value)
                                {
                                    tempDataList = tempFilteredData.OrderBy(pair => pr.GetValue(pair)).Skip(skip).Take(take);
                                }
                                else
                                {
                                    tempDataList = tempFilteredData.OrderByDescending(pair => pr.GetValue(pair)).Skip(skip).Take(take);
                                }
                            }
                            else
                            {
                                tempDataList = tempFilteredData.Skip(skip).Take(take);
                            }

                        }
                        else
                        {
                            if (ascending.HasValue)
                            {
                                if (ascending.Value)
                                {
                                    tempDataList = Data.AsQueryable().OrderBy(pair => pr.GetValue(pair)).Skip(skip).Take(take);
                                }
                                else
                                {
                                    tempDataList = Data.AsQueryable().OrderByDescending(pair => pr.GetValue(pair)).Skip(skip).Take(take);
                                }
                            }
                            else
                            {
                                tempDataList = Data.AsQueryable().Skip(skip).Take(take);
                            }

                        }
                        break;
                    case GridFilterMode.FilterRow:
                        if (filterData.Count() > 0)
                        {
                            var tempFilteredData = Filter(filterData);
                            filteredItemsCount = tempFilteredData.Count();
                            if (ascending.HasValue)
                            {
                                if (ascending.Value)
                                {
                                    tempDataList = tempFilteredData.OrderBy(pair => pr.GetValue(pair)).Skip(skip).Take(take);
                                }
                                else
                                {
                                    tempDataList = tempFilteredData.OrderByDescending(pair => pr.GetValue(pair)).Skip(skip).Take(take);
                                }
                            }
                            else
                            {
                                tempDataList = tempFilteredData.Skip(skip).Take(take);
                            }

                        }
                        else
                        {
                            if (ascending.HasValue)
                            {
                                if (ascending.Value)
                                {
                                    tempDataList = Data.AsQueryable().OrderBy(pair => pr.GetValue(pair)).Skip(skip).Take(take);
                                }
                                else
                                {
                                    tempDataList = Data.AsQueryable().OrderByDescending(pair => pr.GetValue(pair)).Skip(skip).Take(take);
                                }
                            }
                            else
                            {
                                tempDataList = Data.AsQueryable().Skip(skip).Take(take);
                            }

                        }
                        break;
                    default:
                        break;
                }
            }
            return (tempDataList, filteredItemsCount);
        }

        public (IEnumerable<T>, int) GetData(string sortingColumn, bool? ascending, Dictionary<string, FilterCellInfo> filterData, Func<T, bool> customFilter, GridFilterMode filterMode)
        {
            IEnumerable<T> tempDataList = null;
            int filteredItemsCount = Data.Count();

            if (string.IsNullOrEmpty(sortingColumn))
            {
                switch (filterMode)
                {
                    case GridFilterMode.None:
                        tempDataList = Data.AsQueryable();
                        break;
                    case GridFilterMode.Custom:
                        if (customFilter != null)
                        {
                            var tempFilteredData = CustomFilter(customFilter);
                            filteredItemsCount = tempFilteredData.Count();
                            tempDataList = tempFilteredData;
                        }
                        else
                        {
                            tempDataList = Data.AsQueryable();
                        }
                        break;
                    case GridFilterMode.FilterRow:
                        if (filterData.Count() > 0)
                        {
                            var tempFilteredData = Filter(filterData);
                            filteredItemsCount = tempFilteredData.Count();
                            tempDataList = tempFilteredData;
                        }
                        else
                        {
                            tempDataList = Data.AsQueryable();
                        }
                        break;
                    default:
                        break;
                }

            }
            else
            {
                var pr = typeof(T).GetProperty(sortingColumn);
                switch (filterMode)
                {
                    case GridFilterMode.None:
                        if (ascending.HasValue)
                        {
                            if (ascending.Value)
                            {
                                tempDataList = Data.AsQueryable().OrderBy(pair => pr.GetValue(pair));
                            }
                            else
                            {
                                tempDataList = Data.AsQueryable().OrderByDescending(pair => pr.GetValue(pair));
                            }
                        }
                        else
                        {
                            tempDataList = Data.AsQueryable();
                        }

                        break;
                    case GridFilterMode.Custom:
                        if (customFilter != null)
                        {
                            var tempFilteredData = CustomFilter(customFilter);
                            filteredItemsCount = tempFilteredData.Count();
                            if (ascending.HasValue)
                            {
                                if (ascending.Value)
                                {
                                    tempDataList = tempFilteredData.OrderBy(pair => pr.GetValue(pair));
                                }
                                else
                                {
                                    tempDataList = tempFilteredData.OrderByDescending(pair => pr.GetValue(pair));
                                }
                            }
                            else
                            {
                                tempDataList = tempFilteredData;
                            }

                        }
                        else
                        {
                            if (ascending.HasValue)
                            {
                                if (ascending.Value)
                                {
                                    tempDataList = Data.AsQueryable().OrderBy(pair => pr.GetValue(pair));
                                }
                                else
                                {
                                    tempDataList = Data.AsQueryable().OrderByDescending(pair => pr.GetValue(pair));
                                }
                            }
                            else
                            {
                                tempDataList = Data.AsQueryable();
                            }
                        }
                        break;
                    case GridFilterMode.FilterRow:
                        if (filterData.Count() > 0)
                        {
                            var tempFilteredData = Filter(filterData);
                            filteredItemsCount = tempFilteredData.Count();
                            if (ascending.HasValue)
                            {
                                if (ascending.Value)
                                {
                                    tempDataList = tempFilteredData.OrderBy(pair => pr.GetValue(pair));
                                }
                                else
                                {
                                    tempDataList = tempFilteredData.OrderByDescending(pair => pr.GetValue(pair));
                                }
                            }
                            else
                            {
                                tempDataList = tempFilteredData;
                            }

                        }
                        else
                        {
                            if (ascending.HasValue)
                            {
                                if (ascending.Value)
                                {
                                    tempDataList = Data.AsQueryable().OrderBy(pair => pr.GetValue(pair));
                                }
                                else
                                {
                                    tempDataList = Data.AsQueryable().OrderByDescending(pair => pr.GetValue(pair));
                                }
                            }
                            else
                            {
                                tempDataList = Data.AsQueryable();
                            }

                        }
                        break;
                    default:
                        break;
                }
            }
            return (tempDataList, filteredItemsCount);
        }

        public IQueryable<T> CustomFilter(Func<T, bool> customFilter)
        {
            var f = Data.Where(customFilter).AsQueryable();
            var query = from pair in Data.AsQueryable()
                        join filter in f
                             on pair equals filter
                        select pair;

            return query;
        }

        public IQueryable<T> Filter(Dictionary<string, FilterCellInfo> filterData)
        {
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

            Type t = typeof(T);
            ParameterExpression obj = Expression.Parameter(typeof(T));

            List<Expression> lambdaList = new List<Expression>();

            ConditionalExpression isNullExpression = null;

            foreach (var pair in filterData)
            {
                string field = pair.Key;
                FilterCellInfo filterCellInfo = pair.Value;

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

                        switch (pair.Value.FilterCellMode)
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
                        switch (pair.Value.FilterCellMode)
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

            return Data.AsQueryable().Where(lambda);
        }

        public async Task<T> GetDataAtPositionAsync(int position)
        {
            return await Task.Run(() =>
            {
                return Data.ElementAt(position);
            });
        }
        public T GetDataAtPosition(int position)
        {
            return Data.ElementAt(position);
        }

        public async Task NotifyDataLoaded(List<T> data)
        {
            if (OnNotifyDataLoaded != null)
            {
                await OnNotifyDataLoaded.Invoke(data);
            }
        }

        public Task<IEnumerable<T>> GetDataForVirtualPage(int skip, int take)
        {
            return Task.Run(() =>
            {
                return Data.Skip(skip).Take(take);
            });
        }

        public void Dispose()
        {
            Data.Clear();
            Data = null;
        }
    }
}