using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Samovar.DataGrid
{
    internal class GridModelService<T>
    : IGridModelService<T>, IDisposable
    {
        [CascadingParameter(Name = "datagrid-container")]
        protected SamovarGrid<T> DataGrid { get; set; }

        [Inject]
        private IJSRuntime JsRuntime { get; set; }
        public IGridDataRepository<T> Repository { get; set; }

        public List<GridRowModel<T>> ViewCollection { get; set; } = new List<GridRowModel<T>>();

        public Dictionary<string, PropertyInfo> PropInfo { get; } = new Dictionary<string, PropertyInfo>();

        public Dictionary<Guid, ColumnMetadata> ColumnMetadataList { get; set; }

        public Dictionary<string, FilterCellInfo> FilterData { get; } = new Dictionary<string, FilterCellInfo>();
        public Func<T, bool> CustomFilter { get; set; }

        //Count of all data items count to be show
        //with filter applying, but virtualizing
        public int DataItemsCount { get; private set; }
        public int TotalDataItemsCount { get; private set; }
        public int CurrentSelectedItemIndex { get; set; } = -1;
        public Guid CurrentSelectedDataItemId { get; set; } = Guid.Empty;

        public int virtualScroll_VisibleItems { get; set; } = 30;
        public int virtualScroll_ItemsToShow { get; set; } = 30;
        public double virtualScroll_RowHeight { get; set; } = 30d;
        public double virtualScroll_DummyRowHeight { get; set; } = 30d;
        public int virtualScroll_DummyItemsCount { get; set; } = 0;

        public int virtualScroll_TopVisibleDataItemPosition { get; set; } = 1;
        public int virtualScroll_StartGridItemPosition { get; set; } = 1;
        public int virtualScroll_EndGridItemPosition { get; set; } = 1;
        public string innerGridId { get; set; } = $"innergrid{Guid.NewGuid().ToString().Replace("-", "")}";
        public bool Pageable { get; set; }
        public GridRowModel<T> SingleSelectedRowModel { get; set; }
        public T SingleSelectedDataRow { get; set; }
        public IEnumerable<T> MultipleSelectedDataRows { get; set; }

        GridFilterMode filterMode;
        public GridFilterMode FilterMode { get { return filterMode; } set { filterMode = value; } }

        CancellationTokenSource cancellationTokenSource;

        #region ctor
        public GridModelService()
        {
            foreach (PropertyInfo pi in typeof(T).GetProperties())
            {
                PropInfo.Add(pi.Name, pi);
            }
        }
        #endregion

        public void Init(IEnumerable<T> data, Dictionary<Guid, ColumnMetadata> columnMetadataList)
        {
            Repository = new GridDataRepository<T>(data);
            ColumnMetadataList = columnMetadataList;
        }

        public async Task LoadViewCollection(int pageNumber, int pageSize, GridFilterMode filterMode, string sortingColumn = null, bool? ascending = true)
        {
            var pageData = await Repository.GetPageData(FilterData, pageNumber, pageSize, sortingColumn, ascending, CustomFilter, filterMode);
            var tempModelList = CreateRowModelList(pageData.PageData, ColumnMetadataList, PropInfo);

            tempModelList.ToList().ForEach(item =>
            {
                item.RowSelected = false;
            });

            DataItemsCount = pageData.PageDataCount;
            TotalDataItemsCount = pageData.FilteredDataCount;
            ViewCollection = tempModelList;

            cancellationTokenSource?.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            await LoadDataAsync(tempModelList, token);
        }

        public void LoadViewCollection_V6(int currentPage, int pageSize, GridFilterMode filterMode, string sortingColumn = null, bool? ascending = true)
        {
            var pageData = Repository.GetPageData_V6(FilterData, currentPage, pageSize, sortingColumn, ascending, CustomFilter, filterMode);
            var tempModelList = CreateRowModelList(pageData.PageData, ColumnMetadataList, PropInfo);

            tempModelList.ToList().ForEach(item =>
            {
                item.RowSelected = false;
            });

            DataItemsCount = pageData.PageDataCount;
            TotalDataItemsCount = pageData.FilteredDataCount;

            //Load cell data
            LoadDataAsync_V6(tempModelList);

            ViewCollection = tempModelList;
        }

        public async Task LoadVirtualViewCollection(int skip, int take)
        {
            var pageData = await Repository.GetDataForVirtualPage(skip, take);
            ViewCollection = RowModelHelper<T>.CreateRowModelList(pageData, ColumnMetadataList, PropInfo, skip + 1);
        }

        private List<GridRowModel<T>> CreateRowModelList(IEnumerable<T> gridData, Dictionary<Guid, ColumnMetadata> ColumnMetadataList, Dictionary<string, PropertyInfo> PropInfo)
        {
            var retVal = new List<GridRowModel<T>>();
            int rowPosition = 0;
            foreach (var keyDataPair in gridData)
            {
                rowPosition++;
                retVal.Add(new GridRowModel<T>(keyDataPair, ColumnMetadataList, rowPosition, PropInfo));
            }

            return retVal;
        }

        //public void SortData(string sortingColumn, bool ascending, GridFilterMode filterMode)
        //{
        //    InsertItemsByScroll(virtualScroll_DummyItemsCount, virtualScroll_StartGridItemPosition - 1, virtualScroll_EndGridItemPosition - virtualScroll_StartGridItemPosition + 1, sortingColumn, ascending, true, ViewCollection.Where(rml => rml is GridRowModel<T>).Count(), 0, filterMode);
        //}

//        public void InsertItemsByScroll(int shouldDummyItems, int skip, int take, string sortingColumn, bool? ascending, bool scrollToTop, int itemsToRemove, int bottomDummies, GridFilterMode filterMode)
//        {
//            try
//            {
//                lock (lockObj)
//                {
//                    ViewCollection.RemoveAll((item) => item is GridRowDummyModel<T>);

//                    List<GridRowModel<T>> rowModelItemsToRemove = new List<GridRowModel<T>>();

//                    int rmlCnt = ViewCollection.Where(rml => rml is GridRowModel<T>).Count();

//                    if (scrollToTop)
//                    {
//                        for (int i = 1; i <= itemsToRemove; i++)
//                        {
//                            var item = ViewCollection.Where(rml => rml is GridRowModel<T>).ToList()[rmlCnt - i];
//                            rowModelItemsToRemove.Add((GridRowModel<T>)item);
//                        }
//                    }
//                    else
//                    {
//                        for (int i = 0; i < itemsToRemove; i++)
//                        {
//                            try
//                            {
//                                var item = ViewCollection.Where(rml => rml is GridRowModel<T>).ToList()[i];
//                                rowModelItemsToRemove.Add((GridRowModel<T>)item);
//                            }
//                            catch (Exception ex)
//                            {
//#if DEBUG
//                                Debug.WriteLine($"InsertItemsByScroll: {ex.Message}");
//#endif
//                            }
//                        }
//                    }

//                    rowModelItemsToRemove.ForEach(item => item.Dispose());
//                    ViewCollection.RemoveAll((item) => rowModelItemsToRemove.Contains(item));

//                    (IEnumerable<T> PageData, int FilteredDataCount) tempDataList = Repository.GetData(skip, take, sortingColumn, ascending, FilterData, CustomFilter, filterMode);
//                    DataItemsCount = tempDataList.FilteredDataCount;
//                    TotalDataItemsCount = tempDataList.FilteredDataCount;
//                    List<GridRowModel<T>> tempModelList = RowModelHelper<T>.CreateRowModelList(tempDataList.PageData, ColumnMetadataList, PropInfo, skip + 1);

//                    if (scrollToTop)
//                        ViewCollection.InsertRange(0, tempModelList);
//                    else
//                        ViewCollection.AddRange(tempModelList);

//                    if (shouldDummyItems > 0)
//                    {
//                        List<GridRowDummyModel<T>> tempDummyList = RowModelHelper<T>.CreateRowDummyModelList(shouldDummyItems, PropInfo, ColumnMetadataList);
//                        ViewCollection.InsertRange(0, tempDummyList);
//                    }

//                    //compare actualModelLoaderList and tempModelLoaderList
//                    //List<ModelLoader<T>> tempModelLoaderList = new List<ModelLoader<T>>();
//                    //tempModelList.ForEach(m => tempModelLoaderList.Add(new ModelLoader<T>((GridRowModel<T>)m)));

//                    //#if DEBUG
//                    //                    Debug.WriteLine($"ActualModelLoaderList: {actualModelLoaderList.Count()}");
//                    //#endif

//                    //                    actualModelLoaderList.AddRange(tempModelLoaderList);
//                    //#if DEBUG
//                    //                    Debug.WriteLine($"ActualModelLoaderList: {actualModelLoaderList.Count()}");
//                    //#endif
//                    Task.Run(async () =>
//                    {
//                        try
//                        {
//                            await LoadDataAsync3(tempModelList);
//                        }
//                        catch
//                        {
//                            //TODO
//                        }
//                    });
//                }
//            }
//            catch (Exception ex)
//            {
//#if DEBUG
//                Debug.WriteLine($"InsertItemsByScroll: {ex.Message}");
//#endif
//            }
//        }

        async Task LoadDataAsync3(List<GridRowModel<T>> tempModelList)
        {
            //Task.Run(async () =>
            {
                var tasks = new List<Task<GridRowModel<T>>>();
                for (int i = 0; i < tempModelList.Count(); i++)
                {
                    tasks.Add(((GridRowModel<T>)tempModelList[i]).GetLoadTask());
                }

                while (tasks.Count > 0)
                {
                    // Identify the first task that completes.
                    Task<GridRowModel<T>> firstFinishedTask = await Task.WhenAny(tasks);

                    // ***Remove the selected task from the list so that you don't
                    // process it more than once.
                    tasks.Remove(firstFinishedTask);

                    // Await the completed task.
                    try
                    {
                        var rowModel = await firstFinishedTask;
                        await rowModel.RaiseNotifyAfterLoadData();
                    }
                    catch (TaskCanceledException)
                    {
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        async Task LoadDataAsync(List<GridRowModel<T>> tempModelList, CancellationToken token)
        {
            //Task.Run(async () =>
            {
                var tasks = new List<Task<GridRowModel<T>>>();
                for (int i = 0; i < tempModelList.Count(); i++)
                {
                    tasks.Add(ProcessLoadRowDataAsync((GridRowModel<T>)tempModelList[i], token));
                }

                while (tasks.Count > 0)
                {
                    // Identify the first task that completes.
                    Task<GridRowModel<T>> firstFinishedTask = await Task.WhenAny(tasks);

                    // ***Remove the selected task from the list so that you don't
                    // process it more than once.
                    tasks.Remove(firstFinishedTask);

                    // Await the completed task.
                    try
                    {
                        //TODO die erste Ladung darf man nicht abbrechen, zumindest die sichtbar ist
                        //if (token.IsCancellationRequested)
                        //    token.ThrowIfCancellationRequested();
                        var rowModel = await firstFinishedTask;
                        await rowModel.RaiseNotifyAfterLoadData();
                    }
                    catch (TaskCanceledException)
                    {
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception)
                    {
                    }
                    //if (!token.IsCancellationRequested)
                    //{
                    //    cancellationTokenSource.Dispose();
                    //    cancellationTokenSource = null;
                    //}
                }
            }
            //);
        }
        void LoadDataAsync_V6(List<GridRowModel<T>> tempModelList)
        {
            //var retVal = new List<GridRowModel<T>>();
            foreach (var model in tempModelList)
            {
                model.RowModel = new GridRowInnerModel<T>
                {
                    Data = model.dataItem,
                    GridCellModelCollection = model.CreateGridRowCellModelCollection2(model.dataItem)
                };
                model.IsLoaded = true;
            }
        }
        Task<GridRowModel<T>> ProcessLoadRowDataAsync(GridRowModel<T> model, CancellationToken ct)
        {
            return Task.Run(() =>
            {
                model.RowModel = new GridRowInnerModel<T>
                {
                    Data = model.dataItem,
                    GridCellModelCollection = model.CreateGridRowCellModelCollection2(model.dataItem)
                };
                model.IsLoaded = true;
                return model;
            });
        }

        public void Clear()
        {
            Repository.RemoveAllItems();
            ViewCollection.ForEach(rm => rm.Dispose());
            ViewCollection.Clear();
            GC.Collect();
        }

        public void ResetFilter()
        {
            CustomFilter = null;
            FilterData.Clear();
        }

        public void SetFilterData(string fieldName, FilterCellInfo filterCellInfo)
        {
            //delete?
            if (filterCellInfo.FilterCellValue == null)
            {
                if (FilterData.ContainsKey(fieldName))
                {
                    FilterData.Remove(fieldName);
                }
            }
            else
            {
                //add or update
                if (FilterData.ContainsKey(fieldName))
                {
                    FilterData[fieldName] = filterCellInfo;
                }
                else
                {
                    FilterData.Add(fieldName, filterCellInfo);
                }
            }
        }

        public void CreateColumnsMetadata()
        {
            foreach (var prop in PropInfo)
            {
                Guid guid = Guid.NewGuid();
                int absoluteWidth = 0;
                ColumnMetadataWidthInfo widthInfo = new ColumnMetadataWidthInfo();

                ColumnMetadata colMeta = new ColumnMetadata
                {
                    Id = guid,
                    ColumnType = GridColumnType.Data,
                    Title = prop.Key,
                    Field = prop.Key,
                    WidthInfo = widthInfo,
                    VisibleAbsoluteWidthValue = (double)absoluteWidth,
                };

                ColumnMetadataList.Add(guid, colMeta);
            }
        }

        public void InitRepository(IEnumerable<T> data)
        {
            Repository = new GridDataRepository<T>(data);
        }

        public void Dispose()
        {
            ViewCollection.Clear();
            ViewCollection = null;
        }

        public async Task ArrowDown()
        {
            if (CurrentSelectedItemIndex + 1 == DataItemsCount)
            {
                return;
            }

            GridRowModel<T> newSelectedModel = null;

            CurrentSelectedItemIndex++;
            newSelectedModel = (GridRowModel<T>)ViewCollection.Where(rm => rm is GridRowModel<T>).FirstOrDefault(rm => (rm as GridRowModel<T>).DataItemPosition == CurrentSelectedItemIndex + 1);
            ViewCollection.Where(rm => rm is GridRowModel<T>).ToList().ForEach(item => (item as GridRowModel<T>).RowSelected = false);

            if (newSelectedModel != null)
            {
                newSelectedModel.RowSelected = true;

                string bndStr = await JsInteropClasses.IsPartialInView(newSelectedModel.HtmlElementId, innerGridId, DataGrid.jsModule);
                HtmlItemInViewCheckInfo bnd = JsonConvert.DeserializeObject<HtmlItemInViewCheckInfo>(bndStr);

                if (bnd.ProbablyNotInView)
                {
                    if (!Pageable)
                    {
                        await JsRuntime.InvokeVoidAsync("GridFunctions.scrollElementVerticalByValue", innerGridId, (newSelectedModel.DataItemPosition + 1 - virtualScroll_VisibleItems) * virtualScroll_RowHeight);
                    }
                    else
                    {
                        await DataGrid.jsModule.InvokeVoidAsync("scrollToBottom", newSelectedModel.HtmlElementId);
                    }
                }
            }
        }

        public async Task ArrowUp()
        {
            if (CurrentSelectedItemIndex - 1 == -1)
            {
                return;
            }
            GridRowModel<T> newSelectedModel = null;

            CurrentSelectedItemIndex--;
            newSelectedModel = (GridRowModel<T>)ViewCollection.Where(rm => rm is GridRowModel<T>).FirstOrDefault(rm => (rm as GridRowModel<T>).DataItemPosition == CurrentSelectedItemIndex + 1);
            ViewCollection.Where(rm => rm is GridRowModel<T>).ToList().ForEach(item => (item as GridRowModel<T>).RowSelected = false);

            if (newSelectedModel != null)
            {
                newSelectedModel.RowSelected = true;
                string bndStr = await JsInteropClasses.IsPartialInView(newSelectedModel.HtmlElementId, innerGridId, DataGrid.jsModule);
                HtmlItemInViewCheckInfo bnd = JsonConvert.DeserializeObject<HtmlItemInViewCheckInfo>(bndStr);
                if (bnd.ProbablyNotInView)
                {
                    if (!Pageable)
                    {
                        await JsRuntime.InvokeVoidAsync("GridFunctions.scrollElementVerticalByValue", innerGridId, (newSelectedModel.DataItemPosition - 1) * virtualScroll_RowHeight);
                    }
                    else
                    {
                        await DataGrid.jsModule.InvokeVoidAsync("scrollToTop", newSelectedModel.HtmlElementId);
                    }
                }
            }
        }
    }
}
