using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Samovar.DataGrid
{
    public partial class SamovarGrid<TItem>
    {
        [Parameter]
        public TItem SingleSelectedDataRow
        {
            get { return rx.GridModelService.SingleSelectedDataRow; }
            set
            {
                ResetSingleSelection();
                rx.GridModelService.SingleSelectedDataRow = value;
                RowModelSetSelectedIntern2();
            }
        }

        [Parameter]
        public Action<TItem> SingleSelectedDataRowChanged { get; set; }


        [Parameter]
        public IEnumerable<TItem> MultipleSelectedDataRows
        {
            get { return rx.GridModelService.MultipleSelectedDataRows; }
            set
            {
                ResetMultipleSelection();
                rx.GridModelService.MultipleSelectedDataRows = value;
                RowModelSetSelectedIntern2();
            }
        }

        [Parameter]
        public Action<IEnumerable<TItem>> MultipleSelectedDataRowsChanged { get; set; }

        internal void RowModelSetSelectedIntern2()
        {
            switch (SelectionMode)
            {
                case GridSelectionMode.None:

                    break;
                case GridSelectionMode.SingleSelectedDataRow:
                    if (rx.GridModelService.SingleSelectedDataRow == null)
                        return;

                    GridRowModel<TItem> rowModel = (GridRowModel<TItem>)rx.GridModelService.ViewCollection.SingleOrDefault(rm => rm is GridRowModel<TItem> && (rm as GridRowModel<TItem>).dataItem.Equals(rx.GridModelService.SingleSelectedDataRow));
                    if (rowModel != null)
                    {
                        RowModelSetSelectedIntern(rowModel);
                        FireRowSelected(rowModel.dataItem);
                    }
                    break;
                case GridSelectionMode.MultipleSelectedDataRows:
                    if (rx.GridModelService.MultipleSelectedDataRows == null)
                        return;
                    var rowModels = rx.GridModelService.ViewCollection.Where(rm => rm is GridRowModel<TItem> model && MultipleSelectedDataRows.Contains(model.dataItem));
                    foreach (var item in rowModels)
                    {
                        (item as GridRowModel<TItem>).RowSelected = true;
                    }
                    break;
                default:
                    break;
            }
        }

        internal async Task RowSelectedIntern(GridRowEventArgs args, GridRowModel<TItem> selectedModel)
        {
            if (ColWidthChangeManager.IsMouseDown)
                return;

            switch (SelectionMode)
            {
                case GridSelectionMode.None:
                    break;
                case GridSelectionMode.SingleSelectedDataRow:
                    rx.GridModelService.SingleSelectedDataRow = selectedModel.dataItem;
                    SingleSelectedDataRowChanged?.Invoke(rx.GridModelService.SingleSelectedDataRow);
                    RowModelSetSelectedIntern(selectedModel);
                    FireRowSelected(selectedModel.dataItem);
                    break;
                case GridSelectionMode.MultipleSelectedDataRows:
                    if (await JsInteropClasses.IsWindowCtrlKeyDown(jsModule))
                    {
                        if (rx.GridModelService.MultipleSelectedDataRows == null || rx.GridModelService.MultipleSelectedDataRows?.Count() == 0)//initial selection in the multiple selection mode
                        {
                            rx.GridModelService.MultipleSelectedDataRows = (new List<TItem> { selectedModel.dataItem }).AsEnumerable();
                            selectedModel.RowSelected = true;
                        }
                        else
                        {
                            var tList = rx.GridModelService.MultipleSelectedDataRows.ToList();
                            if (!tList.Contains(selectedModel.dataItem))
                            {
                                tList.Add(selectedModel.dataItem);
                                rx.GridModelService.MultipleSelectedDataRows = tList.AsEnumerable();
                                selectedModel.RowSelected = true;
                            }
                            else
                            {
                                tList.Remove(selectedModel.dataItem);
                                rx.GridModelService.MultipleSelectedDataRows = tList.AsEnumerable();
                                selectedModel.RowSelected = false;
                            }
                        }

                        MultipleSelectedDataRowsChanged?.Invoke(rx.GridModelService.MultipleSelectedDataRows);
                    }
                    else if (await JsInteropClasses.IsWindowShiftKeyDown(jsModule))
                    {
                        if (rx.GridModelService.MultipleSelectedDataRows == null || rx.GridModelService.MultipleSelectedDataRows?.Count() == 0)//initial selection in the multiple selection mode
                        {
                            rx.GridModelService.MultipleSelectedDataRows = (new List<TItem> { selectedModel.dataItem }).AsEnumerable();
                            selectedModel.RowSelected = true;
                        }
                        else
                        {
                            var tList = rx.GridModelService.MultipleSelectedDataRows.ToList();

                            var rm = (GridRowModel<TItem>)rx.GridModelService.ViewCollection.SingleOrDefault(x => x.Equals(selectedModel));//. MultipleSelectedDataRows.OrderBy(dr => dr.)
                            if (rm != null)
                            {
                                var initPos = ((GridRowModel<TItem>)rx.GridModelService.ViewCollection.SingleOrDefault(y => (y as GridRowModel<TItem>).dataItem.Equals(rx.GridModelService.MultipleSelectedDataRows.First()))).DataItemPosition;

                                int rmMin = initPos;
                                int rmMax = initPos;
                                rx.GridModelService.MultipleSelectedDataRows.ToList().ForEach(x =>
                                {
                                    var currRm = (GridRowModel<TItem>)rx.GridModelService.ViewCollection.SingleOrDefault(y => (y as GridRowModel<TItem>).dataItem.Equals(x));
                                    if (currRm != null && currRm.DataItemPosition <= rmMin) rmMin = currRm.DataItemPosition;
                                    if (currRm != null && currRm.DataItemPosition >= rmMax) rmMax = currRm.DataItemPosition;

                                });

                                int iFrom = 0, iTo = 0;
                                if (rm.DataItemPosition < rmMin)
                                {
                                    iFrom = rm.DataItemPosition;
                                    iTo = rmMin;
                                }
                                else if (rm.DataItemPosition > rmMax)
                                {
                                    iFrom = rmMax + 1;
                                    iTo = rm.DataItemPosition;
                                }

                                var tempList = rx.GridModelService.ViewCollection.Where(rm => rm is GridRowModel<TItem> && (rm as GridRowModel<TItem>).DataItemPosition >= iFrom && (rm as GridRowModel<TItem>).DataItemPosition <= iTo);
                                tempList.ToList().ForEach(item =>
                                {
                                    (item as GridRowModel<TItem>).RowSelected = true;
                                    if (!tList.Contains((item as GridRowModel<TItem>).dataItem))
                                    {
                                        tList.Add((item as GridRowModel<TItem>).dataItem);
                                    }

                                });
                                rx.GridModelService.MultipleSelectedDataRows = tList.AsEnumerable();
                            }
                        }
                        MultipleSelectedDataRowsChanged?.Invoke(rx.GridModelService.MultipleSelectedDataRows);
                    }
                    else
                    {
                        rx.GridModelService.MultipleSelectedDataRows = (new List<TItem> { selectedModel.dataItem }).AsEnumerable();
                        selectedModel.RowSelected = true;
                        MultipleSelectedDataRowsChanged?.Invoke(rx.GridModelService.MultipleSelectedDataRows);
                    }

                    break;
                default:
                    break;
            }

        }

        private void RowModelSetSelectedIntern(GridRowModel<TItem> selectedModel)
        {
            ResetSingleSelection();
            rx.GridModelService.CurrentSelectedItemIndex = selectedModel.DataItemPosition - 1;
            selectedModel.RowSelected = true;
            StateHasChanged();
        }

        private void ResetSingleSelection()
        {
            if (rx.GridModelService != null)
            {
                rx.GridModelService.ViewCollection.Where(rm => rm is GridRowModel<TItem>).ToList().ForEach(item => (item as GridRowModel<TItem>).RowSelected = false);
                rx.GridModelService.CurrentSelectedDataItemId = Guid.Empty;
                rx.GridModelService.CurrentSelectedItemIndex = -1;
            }
        }

        private void ResetMultipleSelection()
        {
            if (rx.GridModelService != null)
            {
                rx.GridModelService.ViewCollection.Where(rm => rm is GridRowModel<TItem>).ToList().ForEach(item => (item as GridRowModel<TItem>).RowSelected = false);
            }
        }
    }
}
