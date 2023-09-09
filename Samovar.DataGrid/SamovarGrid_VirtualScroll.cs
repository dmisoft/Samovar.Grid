using System;

namespace Samovar.DataGrid
{
    public partial class SamovarGrid<TItem>
    {
        //int virtualScroll_preloadRowCount = 10;

        //protected double virtualScroll_TranslateYOffset = 0d;
        //protected double virtualScroll_TranslateYOffsetAdjust = 0;
        //protected double virtualScroll_ActualTopOffset = 0;
        //protected string virtualScroll_TranslatableDivHeight = "";
        //internal string virtualScroll_TranslatableDivId { get; set; } = $"vstranslatablediv{Guid.NewGuid().ToString().Replace("-", "")}";
        //internal string innerGridId { get; set; } = Guid.NewGuid().ToString();
        //internal string innerGridBodyId { get; } = $"innergridbody{Guid.NewGuid().ToString().Replace("-", "")}";
        

        //private async Task InitVirtualScrolling()
        //{
        //    double tInnerGridHeight = await jsModule.InvokeAsync<double>("getElementHeight", new[] { GridModelService.innerGridId });

        //    GridModelService.virtualScroll_VisibleItems = (int)Math.Round(tInnerGridHeight / GridModelService.virtualScroll_RowHeight, 2, MidpointRounding.AwayFromZero);
        //    int tCnt = GridModelService.virtualScroll_VisibleItems + virtualScroll_preloadRowCount;
        //    GridModelService.virtualScroll_ItemsToShow = tCnt > Data.Count() ? Data.Count() : tCnt;

        //    GridModelService.virtualScroll_EndGridItemPosition = GridModelService.virtualScroll_ItemsToShow;
        //    virtualScroll_TranslateYOffset = -virtualScroll_preloadRowCount * GridModelService.virtualScroll_DummyRowHeight;

        //    GridModelService.ViewCollection.Clear();

        //    int take = GridModelService.virtualScroll_ItemsToShow;
        //    GridModelService.InsertItemsByScroll(virtualScroll_preloadRowCount, 0, take, GridColumnService.SortingColumn, GridColumnService.SortingAscending, true, 0, 0, FilterMode);
        //    virtualScroll_TranslatableDivHeight = $"{(GridModelService.virtualScroll_RowHeight * GridModelService.DataItemsCount).ToString(CultureInfo.InvariantCulture)}px";
        //}

        //        protected void ProcessVirtualScrolling(double scrollTop)
        //        {
        //            //The first top data item to show on the grid
        //            int skip = 0;
        //            int take = 0;
        //            int itemsToRemove = 0;

        //            bool scrollToTop = false;

        //            int new_virtualScroll_StartGridItemPosition = 0;
        //            int new_virtualScroll_EndGridItemPosition = 0;

        //            int newTopDummyItemsCount = 0;
        //            int new_virtualScroll_TopVisibleDataItemPosition = 0;

        //            try
        //            {
        //                double new_virtualScroll_ActualTopOffset = scrollTop;// await GridInnerRef.getElementScrollTop(JsRuntime);

        //                new_virtualScroll_TopVisibleDataItemPosition = (int)Math.Round(new_virtualScroll_ActualTopOffset / GridModelService.virtualScroll_RowHeight) + 1;

        //                //Scrolling Phase A und B
        //                if (new_virtualScroll_TopVisibleDataItemPosition - 1 < virtualScroll_preloadRowCount)
        //                {
        //                    newTopDummyItemsCount = virtualScroll_preloadRowCount - new_virtualScroll_TopVisibleDataItemPosition + 1;
        //                    new_virtualScroll_StartGridItemPosition = 1;
        //                    new_virtualScroll_EndGridItemPosition = Math.Min(new_virtualScroll_TopVisibleDataItemPosition + GridModelService.virtualScroll_ItemsToShow, GridModelService.DataItemsCount);
        //                }
        //                else
        //                {
        //                    new_virtualScroll_StartGridItemPosition = new_virtualScroll_TopVisibleDataItemPosition - virtualScroll_preloadRowCount;
        //                    new_virtualScroll_EndGridItemPosition = Math.Min(new_virtualScroll_StartGridItemPosition + GridModelService.virtualScroll_ItemsToShow + virtualScroll_preloadRowCount - 1, GridModelService.DataItemsCount);// Math.Min(newStartGridItemPosition + virtualScroll_ItemsToShow + preloadRowCount - 1, gridModelService.DataItemsCount);
        //                }

        //                int itemsScrollDelta = new_virtualScroll_TopVisibleDataItemPosition - GridModelService.virtualScroll_TopVisibleDataItemPosition;

        //                if (itemsScrollDelta < 0)
        //                {
        //                    scrollToTop = true;

        //                    if (new_virtualScroll_EndGridItemPosition < GridModelService.virtualScroll_StartGridItemPosition)
        //                    {
        //                        itemsToRemove = GridModelService.ViewCollection.Where(v => v is GridRowModel<TItem>).Count();

        //                        skip = new_virtualScroll_StartGridItemPosition - 1;
        //                        take = new_virtualScroll_EndGridItemPosition - new_virtualScroll_StartGridItemPosition + 1;
        //                    }
        //                    else
        //                    {
        //                        itemsToRemove = GridModelService.virtualScroll_EndGridItemPosition - new_virtualScroll_EndGridItemPosition;
        //                        skip = new_virtualScroll_StartGridItemPosition - 1;
        //                        take = GridModelService.virtualScroll_StartGridItemPosition - new_virtualScroll_StartGridItemPosition;
        //                    }
        //                    GridModelService.virtualScroll_TopVisibleDataItemPosition = new_virtualScroll_TopVisibleDataItemPosition;
        //                    GridModelService.virtualScroll_StartGridItemPosition = new_virtualScroll_StartGridItemPosition;
        //                    GridModelService.virtualScroll_EndGridItemPosition = new_virtualScroll_EndGridItemPosition;
        //                }
        //                else
        //                {
        //                    if (new_virtualScroll_StartGridItemPosition > GridModelService.virtualScroll_EndGridItemPosition)
        //                    {

        //                        itemsToRemove = GridModelService.ViewCollection.Where(v => v is GridRowModel<TItem>).Count();
        //                        skip = new_virtualScroll_StartGridItemPosition - 1;
        //                        take = new_virtualScroll_EndGridItemPosition - new_virtualScroll_StartGridItemPosition + 1;
        //                    }
        //                    else
        //                    {
        //                        itemsToRemove = new_virtualScroll_StartGridItemPosition - GridModelService.virtualScroll_StartGridItemPosition;
        //                        skip = GridModelService.virtualScroll_EndGridItemPosition;
        //                        take = new_virtualScroll_EndGridItemPosition - GridModelService.virtualScroll_EndGridItemPosition;
        //                    }
        //                    GridModelService.virtualScroll_TopVisibleDataItemPosition = new_virtualScroll_TopVisibleDataItemPosition;
        //                    GridModelService.virtualScroll_StartGridItemPosition = new_virtualScroll_StartGridItemPosition;
        //                    GridModelService.virtualScroll_EndGridItemPosition = new_virtualScroll_EndGridItemPosition;
        //                }

        //                int bottomDummies = GridModelService.virtualScroll_ItemsToShow + virtualScroll_preloadRowCount - (GridModelService.virtualScroll_EndGridItemPosition - GridModelService.virtualScroll_StartGridItemPosition + newTopDummyItemsCount + 1);
        //                GridModelService.InsertItemsByScroll(newTopDummyItemsCount, skip, take, GridColumnService.SortingColumn, GridColumnService.SortingAscending, scrollToTop, itemsToRemove, bottomDummies, FilterMode);

        //                virtualScroll_ActualTopOffset = new_virtualScroll_ActualTopOffset;
        //            }
        //            catch (Exception ex)
        //            {
        //#if DEBUG
        //                Debug.WriteLine($"ProcessVirtualScrolling: {ex.Message}");
        //#endif
        //            }
        //            finally
        //            {
        //                GridModelService.virtualScroll_DummyItemsCount = newTopDummyItemsCount;
        //            }
        //#if DEBUG
        //            Debug.WriteLine($"ViewCollection.Count(): {GridModelService.ViewCollection.Count()}");
        //#endif
        //        }
    }
}
