using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public class GridSelectionService<T>
        : IGridSelectionService<T>, IDisposable
    {
        public BehaviorSubject<GridSelectionMode> SelectionMode { get; } = new BehaviorSubject<GridSelectionMode>(GridSelectionMode.None);

        public BehaviorSubject<T> SingleSelectedDataRow { get; } = new BehaviorSubject<T>(default(T));

        public BehaviorSubject<IEnumerable<T>> MultipleSelectedDataRows { get; } = new BehaviorSubject<IEnumerable<T>>(default(IEnumerable<T>));

        public Func<Task> SingleSelectedRowCallback { get; set; }
        
        public Func<Task> MultipleSelectedRowsCallback { get; set; }

        T _singleSelectedDataItem;

        private IJsService _jsService;

        private IRepositoryService<T> _repositoryService;

        public GridSelectionService(IJsService jsService, IRepositoryService<T> repositoryService)
        {
            _jsService = jsService;

            _repositoryService = repositoryService;

            _repositoryService.Data.Subscribe(RepositoryDataSourceChanged);
        }

        //private void RepositoryDataSourceChanged(HashSet<T> set)
        //{
        //    throw new NotImplementedException();
        //}

        private void RepositoryDataSourceChanged(HashSet<T> arg)
        {
            switch (SelectionMode.Value)
            {
                case GridSelectionMode.None:
                    break;
                case GridSelectionMode.SingleSelectedDataRow:
                    if (SingleSelectedDataRow.Value != null && !arg.Any(x => x.Equals(SingleSelectedDataRow.Value))) {
                        SingleSelectedDataRow.OnNext(default(T));

                        SingleSelectedRowCallback?.Invoke();
                    }
                    break;
                case GridSelectionMode.MultipleSelectedDataRows:
                    if (MultipleSelectedDataRows.Value != null && MultipleSelectedDataRows.Value.Count() > 0)
                    {
                        MultipleSelectedDataRows.OnNext(MultipleSelectedDataRows.Value.Intersect(arg));
                        
                        MultipleSelectedRowsCallback?.Invoke();
                    }
                    break;
                default:
                    break;
            }

            //return Task.CompletedTask;
        }

        public async Task OnRowSelected(T dataItem)
        {
            //if (ColWidthChangeManager.IsMouseDown)
            //    return;

            switch (SelectionMode.Value)
            {
                case GridSelectionMode.None:
                    break;
                case GridSelectionMode.SingleSelectedDataRow:
                    if (_singleSelectedDataItem != null && _singleSelectedDataItem.Equals(dataItem))
                    {
                        _singleSelectedDataItem = default(T);
                    }
                    else
                    {
                        _singleSelectedDataItem = dataItem;
                    }

                    if (_singleSelectedDataItem == null)
                    {
                        SingleSelectedDataRow.OnNext(default(T));
                    }
                    else
                    {
                        SingleSelectedDataRow.OnNext(_singleSelectedDataItem);
                    }

                    SingleSelectedRowCallback?.Invoke();

                    break;
                case GridSelectionMode.MultipleSelectedDataRows:
                    if (await _jsService.IsWindowCtrlKeyDown())
                    {
                        if (MultipleSelectedDataRows.Value == null || MultipleSelectedDataRows.Value.Count() == 0)//initial selection in the multiple selection mode
                        {
                            MultipleSelectedDataRows.OnNext(new [] { dataItem });
                        }
                        else
                        {
                            var tList = MultipleSelectedDataRows.Value.ToList();
                            if (!tList.Contains(dataItem))
                            {
                                tList.Add(dataItem);
                                MultipleSelectedDataRows.OnNext(tList.AsEnumerable());
                            }
                            else
                            {
                                tList.Remove(dataItem);
                                MultipleSelectedDataRows.OnNext(tList.AsEnumerable());
                            }
                        }

                        MultipleSelectedRowsCallback?.Invoke();

                    }
                    else if (await _jsService.IsWindowShiftKeyDown())
                    {
                        if (MultipleSelectedDataRows.Value == null || MultipleSelectedDataRows?.Value.Count() == 0)//initial selection in the multiple selection mode
                        {
                            MultipleSelectedDataRows.OnNext(new[] { dataItem });
                        }
                        else
                        {
                            var tList = MultipleSelectedDataRows.Value.ToList();

                            var rm = _repositoryService.ViewCollection.SingleOrDefault(x => x.DataItem.Equals(dataItem));//. MultipleSelectedDataRows.OrderBy(dr => dr.)
                            if (rm != null)
                            {
                                var initPos = _repositoryService.ViewCollection.SingleOrDefault(y => y.DataItem.Equals(MultipleSelectedDataRows.Value.First())).DataItemPosition;

                                int rmMin = initPos;
                                int rmMax = initPos;
                                MultipleSelectedDataRows.Value.ToList().ForEach(x =>
                                {
                                    var currRm = _repositoryService.ViewCollection.SingleOrDefault(y => y.DataItem.Equals(x));
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

                                var tempList = _repositoryService.ViewCollection.Where(rm => rm.DataItemPosition >= iFrom && rm.DataItemPosition <= iTo);
                                tempList.ToList().ForEach(item =>
                                {
                                    //(item as GridRowModel<TItem>).RowSelected = true;
                                    if (!tList.Contains(item.DataItem))
                                    {
                                        tList.Add(item.DataItem);
                                    }

                                });
                                MultipleSelectedDataRows.OnNext(tList.AsEnumerable());
                            }
                        }

                        MultipleSelectedRowsCallback?.Invoke();
                    }
                    else
                    {
                        MultipleSelectedDataRows.OnNext(new[] { dataItem });
                        
                        MultipleSelectedRowsCallback?.Invoke();
                    }

                    break;
                default:
                    break;
            }
        }

        public void Dispose()
        {
            SingleSelectedRowCallback = null;

            MultipleSelectedRowsCallback = null;
        }
    }
}
