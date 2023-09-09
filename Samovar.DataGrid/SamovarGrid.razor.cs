//TODO _
//bei Pageable = false ItemsIntern nicht nutzen. Erstmal prüfen.
using Autofac;
using Autofac.Core;
using CommonServiceLocator;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Samovar.DataGrid.Data;
using Samovar.DataGrid.Data.Interface;
using Samovar.DataGrid.Data.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Samovar.DataGrid
{
    public partial class SamovarGrid<TItem>
        : IDisposable
    {
        #region JS
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }
        public IJSObjectReference jsModule { get; private set; }
        #endregion

        internal string GridBodyContainerId { get; } = $"gridbodycontainer{Guid.NewGuid().ToString().Replace("-", "")}";
        internal string innerGridBodyTableId { get; } = $"innergridbodytable{Guid.NewGuid().ToString().Replace("-", "")}";
        internal string gridBodyId { get; set; } = $"gridbody{Guid.NewGuid().ToString().Replace("-", "")}";
        internal string outerGridId { get; set; } = $"outergrid{Guid.NewGuid().ToString().Replace("-", "")}";
        internal string gridHeaderContainerId { get; set; } = $"gridheadercontainer{Guid.NewGuid().ToString().Replace("-", "")}";
        internal string gridFilterContainerId { get; set; } = $"gridfiltercontainer{Guid.NewGuid().ToString().Replace("-", "")}";
        private string DataGridId { get; } = $"samovargrid{Guid.NewGuid().ToString().Replace("-", "")}";

        //internal IGridModelService<TItem> GridModelService { get; set; } = new GridModelService<TItem>();
        internal ColumnWidthChangeManager ColWidthChangeManager = new ColumnWidthChangeManager();
        internal GridNotifierService NotifierService { get; set; } = new GridNotifierService();
        internal GridColumnService GridColumnService = new GridColumnService();
        internal GridEditingService<TItem> GridEditingService = new GridEditingService<TItem>();
        internal GridColumnDragDropService _ColumnDragDropService = new GridColumnDragDropService();
        internal GridNavigationService NavigationService = new GridNavigationService();
        internal GridGuiStateService GuiStateService = new GridGuiStateService();


        [Parameter]
        public GridSelectionMode SelectionMode { get; set; } = GridSelectionMode.SingleSelectedDataRow;
        [Parameter]
        public string Height { get; set; } = "400px";
        [Parameter]
        public string Width { get; set; } = "100%";
        [Parameter]
        public bool Headless { get; set; }

        GridFilterMode filterMode = GridFilterMode.None;
        [Parameter]
        public GridFilterMode FilterMode
        {
            get
            {
                return filterMode;
            }
            set
            {
                filterMode = value;
                rx.GridModelService.FilterMode = value;
            }
        }

        [Parameter]
        public string TableTagClass { get; set; } = "table table-bordered";
        [Parameter]
        public string FilterToggleButtonClass { get; set; } = "btn btn-secondary";
        [Parameter]
        public string PaginationClass { get; set; } = "pagination";

        [Parameter]
        public string TheadTagClass { get; set; }
        [Parameter]
        public RenderFragment GridColumns { get; set; }

        [Parameter]
        public RenderFragment GridSummaryTemplate { get; set; }

        [Parameter]
        public bool ShowDetailRow { get; set; }
        [Parameter]
        public RenderFragment<TItem> DetailRowTemplate { get; set; }

        [Parameter]
        public GridEditMode EditMode { get; set; } = GridEditMode.Form;

        [Parameter]
        public RenderFragment<TItem> EditFormTemplate { get; set; }
        [Parameter]
        public RenderFragment InsertDataTemplate { get; set; }

        IEnumerable<TItem> data;

        internal Rx<TItem> rx = new Rx<TItem>();

        [Parameter]
        public IEnumerable<TItem> Data
        {
            get
            {
                return data;//TODO DMi return null
            }
            set
            {
                data = value;
                if (GridEditingService.GridState == GridState.Idle)
                {
                    rx.rxQueuePubSub.Enqueue(new Job_LoadGridViewModel<TItem>
                    {
                        Data = data,
                        Grid = this
                        //NavigationService = NavigationService,
                        //ColumnService = GridColumnService
                    });
                }
            }
        }

        [Parameter]
        public string SelectedRowClass { get; set; } = "bg-warning";

        #region Pagination Parameter
        int pageSize_V6 = 10;
        [Parameter]
        public int PageSize
        {
            get { return pageSize_V6; }
            set { NavigationService.PageSize = value; }
        }

        int pagerSize_V5 = 5;
        [Parameter]
        public int PagerSize
        {
            get
            {
                return pagerSize_V5;
            }
            set
            {
                NavigationService.PagerSize = value;
            }
        }
        #endregion

        [Parameter]
        public bool AutoGenerateColumns { get; set; }
        [Parameter]
        public string AutoGenerateColumnsWidth { get; set; } = "150px";

        #region Ordering Parameter
        [Parameter]
        public string OrderFieldByDefault { get; set; }
        [Parameter]
        public bool OrderDesc { get; set; }

        #endregion

        internal double GridColWidthSum = 0;
        internal string InnerGridWidthStyle = "";
        internal double FilterRowHeight = 0;
        public ElementReference GridFilterRef;

        internal ElementReference GridOuterRef { get; set; }
        internal ElementReference GridInnerRef { get; set; }
        internal ElementReference GridFooterRef { get; set; }
        internal ElementReference TableBodyInnerRef { get; set; }

        internal DotNetObjectReference<SamovarGrid<TItem>> DataGridDotNetRef;

        internal double ScrollbarWidth { get; set; }
        internal double ActualScrollbarWidth { get; set; }
        internal bool FitColumnsToTableWidth { get; set; } = false;
        internal double MinGridWidth { get; set; }

        MichelService srv;
        protected override async Task OnInitializedAsync()
        {
            
            srv = ContainerLifetimScope.Resolve<MichelService>();

            //srv = (MichelService)ServiceLocator.Current.GetInstance<IMichelService>();
            //var scope = MichelServiceRegisterer.Container.BeginLifetimeScope(ComponentId);
            //{
            //    srv = (MichelService)scope.Resolve<IMichelService>();
            //}
            NotifierService.NotifyAfterGridResize += NotifierService_NotifyAfterGridResize;

            await InvokeAsync(() =>
            {
                OuterStyle = $"height:{Height};";

                if (!string.IsNullOrEmpty(Width))
                {
                    OuterStyle += $"width:{Width};";
                    FooterStyle = $"width:{Width};";
                }

                GridColumnService.Init(OrderFieldByDefault, OrderDesc);
                GridEditingService.Init(this);
                rx.GridModelService.Init(Data, GridColumnService.Columns);

                if (AutoGenerateColumns)
                    ProcessAutogenerateColumns();

                if (Data is ObservableCollection<TItem>)
                {
                    (Data as ObservableCollection<TItem>).CollectionChanged += SamovarGrid_CollectionChanged;
                }
            });
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                rx.RegisterHandler();

                jsModule = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/SamovarGrid/samovar.grid.js");

                DataGridDotNetRef = DotNetObjectReference.Create(this);

                await GridInnerRef.SynchronizeGridHeaderScroll(jsModule, gridHeaderContainerId);
                if (rx.GridModelService.FilterMode == GridFilterMode.FilterRow)
                {
                    await GridInnerRef.SynchronizeGridHeaderScroll(jsModule, gridFilterContainerId);
                }

                //await JsRuntime.InvokeVoidAsync("GridFunctions.add_GridInner_OnScroll_EventListener", rx.GridModelService.innerGridId, DataGridDotNetRef);
                //await JsRuntime.InvokeVoidAsync("GridFunctions.add_GridBody_KeyDown_EventListener", rx.GridModelService.innerGridId, dotNetRef, nameof(Js_GridBody_KeyDown));

                await jsModule.InvokeVoidAsync("add_Window_OnResize_EventListener", DataGridId, DataGridDotNetRef);
                ScrollbarWidth = await JsInteropClasses.MeasureScrollbar(jsModule);
                FilterRowHeight = await JsInteropClasses.MeasureTableFilterHeight(jsModule, TableTagClass, TheadTagClass, FilterToggleButtonClass, $"measure{Guid.NewGuid().ToString().Replace("-", "")}");

                await InitHeader();

                rx.rxQueuePubSub.Enqueue(new Job_CalculateInittialPageInfo<TItem>
                {
                    NavigationService = NavigationService,
                    DataCount = data.Count()
                });

                rx.rxQueuePubSub.Enqueue(new Job_LoadGridViewModel<TItem>
                {
                    Data = data,
                    Grid = this
                });

                StateHasChanged();
            }
            else
            {
                if (await CheckScrollBarWidth())
                    StateHasChanged();
                //else if (!isInitScrolbarVisibilityChecked)
                //{
                //    isInitScrolbarVisibilityChecked = true;
                //    StateHasChanged();
                //}
            }
        }

        public IEnumerable<TItem> ViewCollection
        {
            get
            {
                return rx.GridModelService.Repository.GetData(GridColumnService.SortingColumn, GridColumnService.SortingAscending, rx.GridModelService.FilterData, rx.GridModelService.CustomFilter, rx.GridModelService.FilterMode).Item1.Select(i => i);
            }
        }

        /// <summary>
        /// ObservableCollection handling
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SamovarGrid_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            (Data as ObservableCollection<TItem>).CollectionChanged -= SamovarGrid_CollectionChanged;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    rx.GridModelService.Repository.InsertItems(e.NewItems, e.NewStartingIndex);
                    InvokeAsync(InitialLoad);
                    StateHasChanged();
                    break;
                case NotifyCollectionChangedAction.Remove:
                    rx.GridModelService.Repository.RemoveItems(e.OldItems);
                    InvokeAsync(InitialLoad);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    rx.GridModelService.Repository.RemoveItems(e.OldItems);
                    rx.GridModelService.Repository.InsertItems(e.NewItems, e.NewStartingIndex);
                    InvokeAsync(InitialLoad);
                    break;
                case NotifyCollectionChangedAction.Move:
                    var temp = rx.GridModelService.Repository.GetDataAtPosition(e.OldStartingIndex);
                    rx.GridModelService.Repository.Data[e.OldStartingIndex] = rx.GridModelService.Repository.Data[e.NewStartingIndex];
                    rx.GridModelService.Repository.Data[e.NewStartingIndex] = temp;
                    InvokeAsync(InitialLoad);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    InvokeAsync(rx.GridModelService.Clear);
                    InvokeAsync(InitialLoad);
                    break;
            }
            (Data as ObservableCollection<TItem>).CollectionChanged += SamovarGrid_CollectionChanged;
        }

        internal void UpdateItemsForActualPage(int currentPage)
        {
            rx.rxQueuePubSub.Enqueue(new Job_NavigateToSelectedPage<TItem>
            {
                Page = currentPage,
                NavigationService = NavigationService
            });

            rx.rxQueuePubSub.Enqueue(new Job_LoadGridViewModel<TItem>
            {
                Data = data,
                Grid = this
                //NavigationService = NavigationService,
                //ColumnService = GridColumnService
            });


            //bool fireAfterPagerChange = (NavigationService.CurrentPage != currentPage);
            //await rx.GridModelService.LoadViewCollection(currentPage, NavigationService.PageSize, FilterMode, GridColumnService.SortingColumn, GridColumnService.SortingAscending);

            //NavigationService.CurrentPage = currentPage;
            //if (fireAfterPagerChange)
            //{
            //    await AfterPageChange.InvokeAsync(new GridPagingEventArgs(currentPage));
            //    await NotifierService.AfterPagingChange();
            //}

            //RowModelSetSelectedIntern2();
        }

        //internal void UpdateItemsForActualPage_V6(int currentPage)
        //{
        //    bool fireAfterPagerChange = (NavigationService.CurrentPage != currentPage);
        //    rx.GridModelService.LoadViewCollection_V6(currentPage, NavigationService.PageSize, rx.GridModelService.FilterMode, GridColumnService.SortingColumn, GridColumnService.SortingAscending);

        //    NavigationService.CurrentPage = currentPage;
        //    if (fireAfterPagerChange)
        //    {
        //        //await AfterPageChange.InvokeAsync(new GridPagingEventArgs(currentPage));
        //        //await NotifierService.AfterPagingChange();
        //    }

        //    RowModelSetSelectedIntern2();
        //}

        internal void NavigateToPage(string direction)
        {
            rx.rxQueuePubSub.Enqueue(new Job_NavigateToPage<TItem>
            {
                Direction = direction,
                NavigationService = NavigationService
            });

            rx.rxQueuePubSub.Enqueue(new Job_LoadGridViewModel<TItem>
            {
                Data = data,
                Grid = this
                //NavigationService = NavigationService,
                //ColumnService = GridColumnService
            });
        }

        internal void SetPagerSize(string direction)
        {
            rx.rxQueuePubSub.Enqueue(new Job_SetPagerSize<TItem>
            {
                Direction = direction,
                NavigationService = NavigationService
            });

            rx.rxQueuePubSub.Enqueue(new Job_LoadGridViewModel<TItem>
            {
                Data = data,
                Grid = this
                //NavigationService = NavigationService,
                //ColumnService = GridColumnService
            });
        }

        internal async Task ColumnCellClick(string column)
        {
            if (GridColumnService.SortingAscending == null)
            {
                GridColumnService.SortingAscending = true;
            }
            else if (GridColumnService.SortingAscending.Value)
            {
                GridColumnService.SortingAscending = false;
            }
            else
            {
                GridColumnService.SortingAscending = null;
            }

            GridColumnService.SortingColumn = column;

            foreach (var cm in GridColumnService.Columns.Values.Where(cm => cm.ColumnType == GridColumnType.Data))
            {
                cm.SortingAscending = null;
            }

            ColumnMetadata meta = GridColumnService.Columns.Values.FirstOrDefault(cm => cm.ColumnType == GridColumnType.Data && cm.Field == column);

            if (meta != null)
            {
                meta.SortingAscending = GridColumnService.SortingAscending;
            }

            rx.rxQueuePubSub.Enqueue(new Job_LoadGridViewModel<TItem>
            {
                Data = data,
                Grid = this
                //NavigationService = NavigationService,
                //ColumnService = GridColumnService
            });
            await NotifierService.AfterSort();

            await AfterSort.InvokeAsync(new GridSortEventArgs(column));
        }

        public async Task RefreshAsync()
        {
            if (Data is ObservableCollection<TItem>)
            {
                (Data as ObservableCollection<TItem>).CollectionChanged -= SamovarGrid_CollectionChanged;
            }

            GridColWidthSum = 0;

            rx.GridModelService.CurrentSelectedDataItemId = Guid.Empty;
            rx.GridModelService.CurrentSelectedItemIndex = -1;
            NavigationService.StartPage = 0;
            NavigationService.EndPage = 0;
            NavigationService.TotalPages = 0;
            NavigationService.CurrentPage = 1;

            rx.GridModelService.Clear();
            rx.GridModelService.InitRepository(Data);

            await InitialLoad();
            StateHasChanged();

            if (Data is ObservableCollection<TItem>)
            {
                (Data as ObservableCollection<TItem>).CollectionChanged += SamovarGrid_CollectionChanged;
            }
        }

        private void ProcessAutogenerateColumns()
        {
            foreach (var prop in rx.GridModelService.PropInfo)
            {
                Guid guid = Guid.NewGuid();
                int absoluteWidth = 150;
                ColumnMetadataWidthInfo widthInfo = new ColumnMetadataWidthInfo();
                string _width = AutoGenerateColumnsWidth;
                bool isAbsoluteWidth = !string.IsNullOrEmpty(_width) && Regex.IsMatch(_width, "^[^0][0-9]*px$");
                bool isRelativeWidth = !string.IsNullOrEmpty(_width) && Regex.IsMatch(_width, @"^[^0][0-9]*\*$");
                if (isAbsoluteWidth)
                {
                    widthInfo.WidthMode = ColumnMetadataWidthInfo.ColumnWidthMode.Absolute;
                    widthInfo.WidthValue = int.Parse(_width.Replace("px", ""));
                }
                else if (isRelativeWidth)
                {
                    widthInfo.WidthMode = ColumnMetadataWidthInfo.ColumnWidthMode.Relative;
                    widthInfo.WidthValue = int.Parse(_width.Replace("*", ""));
                }

                ColumnMetadata colMeta = new ColumnMetadata
                {
                    Id = guid,
                    ColumnType = GridColumnType.Data,
                    Title = prop.Key,
                    Field = prop.Key,
                    WidthInfo = widthInfo,
                    VisibleAbsoluteWidthValue = (double)absoluteWidth,
                };
                if (!string.IsNullOrEmpty(OrderFieldByDefault) && prop.Key == OrderFieldByDefault)
                {
                    colMeta.SortingAscending = !OrderDesc;
                }
                GridColumnService.Columns.Add(guid, colMeta);
                GridColumnService.CellTemplateList.Add(guid, new CellTemplateInfo { CellShowTemplate = null, CellEditTemplate = null });
            }
            rx.GridModelService.ColumnMetadataList = GridColumnService.Columns;
        }

        internal async Task<bool> CheckScrollBarWidth()
        {
            try
            {
                double tempActualScrollbarWidth = 0d;

                if (await jsModule.InvokeAsync<bool>("isScrollbarVisible", rx.GridModelService.innerGridId))
                {
                    tempActualScrollbarWidth = ScrollbarWidth;
                }

                var retVal = tempActualScrollbarWidth != ActualScrollbarWidth ? true : false;
                ActualScrollbarWidth = tempActualScrollbarWidth;
                return retVal;
            }
            catch
            {
            }

            return false;
        }

        private async Task NotifierService_NotifyAfterGridResize()
        {
            try
            {
                NotifierService.NotifyAfterGridResize -= NotifierService_NotifyAfterGridResize;

                onResizeCancellationTokenSource?.Cancel();
                onResizeCancellationTokenSource = new CancellationTokenSource();
                var token = onResizeCancellationTokenSource.Token;
                if (!token.IsCancellationRequested)
                {
                    var tBodyWidth = await GridOuterRef.GetElementWidthByRef(jsModule);
                    CalculateEmptyColumn(tBodyWidth);

                    if (!token.IsCancellationRequested)
                    {
                        onResizeCancellationTokenSource = null;
                    }
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                NotifierService.NotifyAfterGridResize += NotifierService_NotifyAfterGridResize;
            }
        }

        [JSInvokable]
        public async Task Js_Window_MouseUp(string colMetaId, double newVisibleAbsoluteWidthValue, string emptyColumnId, double emptyColWidth)
        {
            await jsModule.InvokeVoidAsync("remove_Window_MouseMove_EventListener");
            await jsModule.InvokeVoidAsync("remove_Window_MouseUp_EventListener");
            if (!string.IsNullOrEmpty(colMetaId))
                GridColumnService.Columns[new Guid(colMetaId)].VisibleAbsoluteWidthValue = newVisibleAbsoluteWidthValue;

            if (!string.IsNullOrEmpty(emptyColumnId))
                GridColumnService.Columns[new Guid(emptyColumnId)].VisibleAbsoluteWidthValue = emptyColWidth;

            ColWidthChangeManager.Reset();
            ColWidthChanged();
            StateHasChanged();
        }

        [JSInvokable]
        public async Task JS_AfterWindowResize()
        {
            var tBodyWidth = await GridOuterRef.GetElementWidthByRef(jsModule);
            CalculateEmptyColumn(tBodyWidth);
            StateHasChanged();
        }

        internal void ColWidthChanged()
        {
            //TODO
            if (!FitColumnsToTableWidth)
            {
                double newGridColWidthSum = 0;
                foreach (var m in GridColumnService.Columns.Where(c => c.Value.ColumnType != GridColumnType.EmptyColumn))
                {
                    newGridColWidthSum += m.Value.VisibleAbsoluteWidthValue;
                }
                GridColWidthSum = newGridColWidthSum;
            }
        }

        private async Task InitHeader()
        {
            GridColWidthSum = 0;
            MinGridWidth = 0;

            var allCols = GridColumnService.Columns.Where(c => c.Value.ColumnType != GridColumnType.EmptyColumn).Count();
            var absCols = GridColumnService.Columns.Where(c => c.Value.ColumnType != GridColumnType.EmptyColumn && c.Value.WidthInfo.WidthMode == ColumnMetadataWidthInfo.ColumnWidthMode.Absolute).Count();
            var relCols = GridColumnService.Columns.Where(c => c.Value.ColumnType != GridColumnType.EmptyColumn && c.Value.WidthInfo.WidthMode == ColumnMetadataWidthInfo.ColumnWidthMode.Relative).Count();
            if (absCols != allCols)
            {
                FitColumnsToTableWidth = true;
                var absColsWidthSum = GridColumnService.Columns.Where(c => c.Value.ColumnType != GridColumnType.EmptyColumn && c.Value.WidthInfo.WidthMode == ColumnMetadataWidthInfo.ColumnWidthMode.Absolute).Sum(c => c.Value.WidthInfo.WidthValue);
                MinGridWidth = absColsWidthSum + GridColumnService.Columns.Where(c => c.Value.ColumnType != GridColumnType.EmptyColumn && c.Value.WidthInfo.WidthMode == ColumnMetadataWidthInfo.ColumnWidthMode.Relative).Sum(c => c.Value.WidthInfo.MinWidthValue); ;
            }
            else
            {
                GridColWidthSum = GridColumnService.Columns.Where(c => c.Value.ColumnType != GridColumnType.EmptyColumn).Sum(c => c.Value.WidthInfo.WidthValue);
            }

            if (FitColumnsToTableWidth)
                await ShowDynamicHeader(0);
            else
                await ShowFixHeader(0);
        }

        private async Task ShowDynamicHeader(int newWidth)
        {
            Dictionary<Guid, ColumnMetadata> cloneColumnMetadataList = new Dictionary<Guid, ColumnMetadata>();

            try
            {
                CultureInfo ci = CultureInfo.InvariantCulture;

                double gridInnerWidth = await GridInnerRef.GetElementWidthByRef(jsModule);
                gridInnerWidth = Math.Max(gridInnerWidth, MinGridWidth);

                Dictionary<Guid, TempColumnMetadata> widthList = new Dictionary<Guid, TempColumnMetadata>();

                var absoluteWidthSum = GridColumnService.Columns.Where(cmt => cmt.Value.ColumnType != GridColumnType.EmptyColumn && cmt.Value.WidthInfo.WidthMode == ColumnMetadataWidthInfo.ColumnWidthMode.Absolute).Sum(cmt => cmt.Value.WidthInfo.WidthValue);
                var relativeWidthSum = GridColumnService.Columns.Where(cmt => cmt.Value.ColumnType != GridColumnType.EmptyColumn && cmt.Value.WidthInfo.WidthMode == ColumnMetadataWidthInfo.ColumnWidthMode.Relative).Sum(cmt => cmt.Value.WidthInfo.WidthValue);

                var restForRelative = gridInnerWidth - absoluteWidthSum;
                var restForRelativePercent = (gridInnerWidth - absoluteWidthSum) / gridInnerWidth;

                foreach (var m in GridColumnService.Columns.Where(cmt => cmt.Value.ColumnType != GridColumnType.EmptyColumn && cmt.Value.WidthInfo.WidthMode == ColumnMetadataWidthInfo.ColumnWidthMode.Relative))
                {
                    double nw = restForRelativePercent * m.Value.WidthInfo.WidthValue / relativeWidthSum;
                    widthList.Add(m.Key, new TempColumnMetadata { VisibleAbsoluteWidthValue = nw * gridInnerWidth, VisiblePercentWidthValue = nw * 100d });
                }

                foreach (var m in GridColumnService.Columns.Where(cmt => cmt.Value.ColumnType != GridColumnType.EmptyColumn && cmt.Value.WidthInfo.WidthMode == ColumnMetadataWidthInfo.ColumnWidthMode.Absolute))
                {
                    double nw = m.Value.WidthInfo.WidthValue / gridInnerWidth;
                    widthList.Add(m.Key, new TempColumnMetadata { VisibleAbsoluteWidthValue = m.Value.WidthInfo.WidthValue, VisiblePercentWidthValue = nw * 100d });
                }
                //Test
                var abs = widthList.Sum(cmt => cmt.Value.VisibleAbsoluteWidthValue);
                var rel = widthList.Sum(cmt => cmt.Value.VisiblePercentWidthValue);

                //Werte aus TempObjekt transferieren
                foreach (var m in GridColumnService.Columns)
                {
                    m.Value.VisibleAbsoluteWidthValue = widthList[m.Key].VisibleAbsoluteWidthValue;
                    m.Value.VisiblePercentWidthValue = widthList[m.Key].VisiblePercentWidthValue;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task ShowFixHeader(int newWidth)
        {
            Dictionary<Guid, ColumnMetadata> cloneColumnMetadataList = new Dictionary<Guid, ColumnMetadata>();

            try
            {
                CultureInfo ci = CultureInfo.InvariantCulture;

                double gridInnerWidth = await GridInnerRef.GetElementWidthByRef(jsModule);
                Dictionary<Guid, TempColumnMetadata> widthList = new Dictionary<Guid, TempColumnMetadata>();

                foreach (var m in GridColumnService.Columns.Where(cmt => cmt.Value.ColumnType != GridColumnType.EmptyColumn && cmt.Value.WidthInfo.WidthMode == ColumnMetadataWidthInfo.ColumnWidthMode.Absolute))
                {
                    double nw = m.Value.WidthInfo.WidthValue / gridInnerWidth;
                    widthList.Add(m.Key, new TempColumnMetadata { VisibleAbsoluteWidthValue = m.Value.WidthInfo.WidthValue, VisiblePercentWidthValue = nw * 100d });
                }

                //Werte aus TempObjekt transferieren
                foreach (var m in GridColumnService.Columns)
                {
                    if (widthList.ContainsKey(m.Key))
                    {
                        m.Value.VisibleAbsoluteWidthValue = widthList[m.Key].VisibleAbsoluteWidthValue;
                        m.Value.VisiblePercentWidthValue = widthList[m.Key].VisiblePercentWidthValue;
                    }
                }

                double tBodyWidth = 0;
                if (newWidth == 0)
                    tBodyWidth = await GridOuterRef.GetElementWidthByRef(jsModule);
                else
                    tBodyWidth = newWidth;

                CalculateEmptyColumn(tBodyWidth);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void CalculateEmptyColumn(double tBodyWidth)
        {
            double emptyColWidth = 0;
            if (!FitColumnsToTableWidth)
            {
                emptyColWidth = tBodyWidth - GridColWidthSum - ScrollbarWidth - 1;
                emptyColWidth = Math.Max(0, emptyColWidth);
            }

            var emptyCol = GridColumnService.Columns.Values.Where(cm => cm.ColumnType == GridColumnType.EmptyColumn).FirstOrDefault();
            if (emptyCol == null)
            {
                Guid emptyColGuid = Guid.NewGuid();
                ColumnMetadata emptyColMeta = new ColumnMetadata
                {
                    Id = emptyColGuid,
                    Title = "",
                    VisibleAbsoluteWidthValue = emptyColWidth,
                    ColumnType = GridColumnType.EmptyColumn,
                    WidthInfo = new ColumnMetadataWidthInfo { WidthMode = ColumnMetadataWidthInfo.ColumnWidthMode.Absolute, WidthValue = emptyColWidth },
                    ColumnOrder = int.MaxValue
                };
                GridColumnService.Columns.Add(emptyColGuid, emptyColMeta);
            }
            else
            {
                emptyCol.VisibleAbsoluteWidthValue = emptyColWidth;
                emptyCol.ColumnType = GridColumnType.EmptyColumn;
                emptyCol.WidthInfo = new ColumnMetadataWidthInfo { WidthMode = ColumnMetadataWidthInfo.ColumnWidthMode.Absolute, WidthValue = emptyColWidth };
            }
        }

        //private void CalculateInitPaginationInfo()
        //{
        //    CurrentPage = 0;
        //    StartPage = 0;
        //    EndPage = 0;
        //    TotalPages = 0;

        //    if (Data.Count() > 0)
        //    {
        //        CurrentPage = 1;
        //        TotalPages = (int)Math.Ceiling(Data.Count() / (decimal)PageSize);
        //    }
        //}

        private async Task InitialLoad()
        {
            NavigationService.CalculateInitPaginationInfo(data.Count());
            SetPagerSize("forward");
            UpdateItemsForActualPage(NavigationService.CurrentPage);
        }

        private void SetSelectedStyle()
        {
            rx.GridModelService.ViewCollection.Where(rm => rm is GridRowModel<TItem> && (rm as GridRowModel<TItem>).DataItemPosition == rx.GridModelService.CurrentSelectedItemIndex + 1).ToList().ForEach(item => (item as GridRowModel<TItem>).RowSelected = true);
        }

        CancellationTokenSource onResizeCancellationTokenSource;

        //[JSInvokable]
        public async Task Js_GridBody_KeyDown(KeyboardEventArgs args)
        {
            GridRowModel<TItem> newSelectedModel = null;
            string scrollFunction;
            try
            {
                if (args.Key == "ArrowDown")
                {
                    if (rx.GridModelService.CurrentSelectedItemIndex + 1 == rx.GridModelService.DataItemsCount)
                    {
                        return;
                    }
                    scrollFunction = "scrollToBottom";
                    int add = 1;
                    rx.GridModelService.CurrentSelectedItemIndex += add;
                    newSelectedModel = (GridRowModel<TItem>)rx.GridModelService.ViewCollection.Where(rm => rm is GridRowModel<TItem>).FirstOrDefault(rm => (rm as GridRowModel<TItem>).DataItemPosition == rx.GridModelService.CurrentSelectedItemIndex + Math.Abs(add));
                    rx.GridModelService.ViewCollection.Where(rm => rm is GridRowModel<TItem>).ToList().ForEach(item => (item as GridRowModel<TItem>).RowSelected = false);

                    if (newSelectedModel != null)
                    {
                        newSelectedModel.RowSelected = true;
                        string bndStr = await JsInteropClasses.IsPartialInView(newSelectedModel.HtmlElementId, rx.GridModelService.innerGridId, jsModule);
                        HtmlItemInViewCheckInfo bnd = JsonConvert.DeserializeObject<HtmlItemInViewCheckInfo>(bndStr);

                        if (bnd.ProbablyNotInView)
                        {
                            await jsModule.InvokeVoidAsync(scrollFunction, newSelectedModel.HtmlElementId);
                        }

                        FireRowSelected(newSelectedModel.dataItem);
                    }
                }
                else if (args.Key == "ArrowUp")
                {
                    if (rx.GridModelService.CurrentSelectedItemIndex - 1 == -1)
                    {
                        return;
                    }
                    scrollFunction = "scrollToTop";
                    int add = -1;
                    rx.GridModelService.CurrentSelectedItemIndex += add;
                    newSelectedModel = (GridRowModel<TItem>)rx.GridModelService.ViewCollection.Where(rm => rm is GridRowModel<TItem>).FirstOrDefault(rm => (rm as GridRowModel<TItem>).DataItemPosition == rx.GridModelService.CurrentSelectedItemIndex + Math.Abs(add));
                    rx.GridModelService.ViewCollection.Where(rm => rm is GridRowModel<TItem>).ToList().ForEach(item => (item as GridRowModel<TItem>).RowSelected = false);

                    if (newSelectedModel != null)
                    {
                        newSelectedModel.RowSelected = true;
                        string bndStr = await JsInteropClasses.IsPartialInView(newSelectedModel.HtmlElementId, rx.GridModelService.innerGridId, jsModule);
                        HtmlItemInViewCheckInfo bnd = JsonConvert.DeserializeObject<HtmlItemInViewCheckInfo>(bndStr);
                        if (bnd.ProbablyNotInView)
                        {
                            await jsModule.InvokeVoidAsync(scrollFunction, newSelectedModel.HtmlElementId);
                        }

                        FireRowSelected(newSelectedModel.dataItem);
                    }
                }
                else if (args.Key == "PageDown")
                {
                    var newTopVisibleItemPosition = 0;
                    if (rx.GridModelService.virtualScroll_TopVisibleDataItemPosition + rx.GridModelService.virtualScroll_VisibleItems * 2 >= rx.GridModelService.DataItemsCount)
                    {
                        newTopVisibleItemPosition = rx.GridModelService.DataItemsCount - rx.GridModelService.virtualScroll_VisibleItems + 1;
                    }
                    else
                    {
                        newTopVisibleItemPosition = rx.GridModelService.virtualScroll_TopVisibleDataItemPosition + rx.GridModelService.virtualScroll_VisibleItems;
                    }

                    await jsModule.InvokeVoidAsync("scrollToTop", newSelectedModel.HtmlElementId);
                }
                else if (args.Key == "PageUp")
                {
                    var newTopVisibleItemPosition = 0;
                    if (rx.GridModelService.virtualScroll_TopVisibleDataItemPosition - rx.GridModelService.virtualScroll_VisibleItems * 2 <= 1)
                    {
                        newTopVisibleItemPosition = 1;
                    }
                    else
                    {
                        newTopVisibleItemPosition = rx.GridModelService.virtualScroll_TopVisibleDataItemPosition - rx.GridModelService.virtualScroll_VisibleItems;
                    }

                    await jsModule.InvokeVoidAsync("scrollToTop", newSelectedModel.HtmlElementId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                SetSelectedStyle();
            }
        }

        internal async Task Filter(ColumnMetadata colMeta, FilterCellInfo filterCellInfo)
        {
            rx.GridModelService.SetFilterData(colMeta.Field, filterCellInfo);
            ProcessFilter();
        }

        public void ResetOrder()
        {
            OrderFieldByDefault = string.Empty;
            GridColumnService.ResetOrder();
        }

        public void ResetFilterAsync()
        {
            rx.GridModelService.ResetFilter();
            ProcessFilter();
        }

        internal void ProcessFilter()
        {
            rx.rxQueuePubSub.Enqueue(new Job_LoadInitFilteredViewModel<TItem>
            {
                Data = data,
                Grid = this
            });
        }

        public void Dispose()
        {
            if (Data is ObservableCollection<TItem>)
            {
                (Data as ObservableCollection<TItem>).CollectionChanged -= SamovarGrid_CollectionChanged;
            }
            NotifierService.NotifyAfterGridResize -= NotifierService_NotifyAfterGridResize;

            ((IDisposable)rx.GridModelService.Repository)?.Dispose();
            Task.Run(() => jsModule.InvokeVoidAsync("disposeDataGridInstance", DataGridId));
            DataGridDotNetRef?.Dispose();
        }

        public async Task ScrollToPosition(int position)
        {
            if (position <= rx.GridModelService.DataItemsCount)
            {
                await JsRuntime.InvokeVoidAsync("GridFunctions.scrollElementVerticalByValue", rx.GridModelService.innerGridId, rx.GridModelService.virtualScroll_RowHeight * (position - 1));
            }
        }

        internal ValueTask<double> GetInnerWidth()
        {
            return GridInnerRef.GetElementWidthByRef(jsModule);
        }

        internal async Task Repaint()
        {
            await InvokeAsync(StateHasChanged);
        }

        public async Task CancelRowEdit()
        {
            await GridEditingService.CancelTemplateRowEdit();
            //StateHasChanged();
        }

        public async Task EndRowInserting()
        {
            await GridEditingService.CancelTemplateRowInserting();
            await InvokeAsync(StateHasChanged);
        }

        public async Task ApplyCustomFilter(Func<TItem, bool> customFilter)
        {
            rx.GridModelService.CustomFilter = customFilter;
            ProcessFilter();
        }

        public void Refresh()
        {
            
            InvokeAsync(StateHasChanged);
        }
    }
    public class InjectablePropertySelector
        : DefaultPropertySelector
    {
        public InjectablePropertySelector(bool preserveSetValues) : base(preserveSetValues)
        { }

        public override bool InjectProperty(PropertyInfo propertyInfo, object instance)
        {
            var attr = propertyInfo.GetCustomAttribute<InjectAttribute>(inherit: true);
            return attr != null && propertyInfo.CanWrite
                    && (!PreserveSetValues
                    || (propertyInfo.CanRead && propertyInfo.GetValue(instance, null) == null));
        }
    }
}
