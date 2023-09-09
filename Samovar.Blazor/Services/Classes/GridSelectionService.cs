using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public class GridSelectionService<T>
        : IGridSelectionService<T>, IDisposable
    {
        public ISubject<GridSelectionMode> SelectionMode { get; } = new ParameterSubject<GridSelectionMode>(GridSelectionMode.None);

        public ISubject<T> SingleSelectedDataRow { get; } = new ParameterSubject<T>(default(T));

        public ISubject<IEnumerable<T>> MultipleSelectedDataRows { get; } = new ParameterSubject<IEnumerable<T>>(default(IEnumerable<T>));

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

        private Task RepositoryDataSourceChanged(IEnumerable<T> arg)
        {
            switch (SelectionMode.SubjectValue)
            {
                case GridSelectionMode.None:
                    break;
                case GridSelectionMode.SingleSelectedDataRow:
                    if (SingleSelectedDataRow.SubjectValue != null && !arg.Any(x => x.Equals(SingleSelectedDataRow.SubjectValue))) {
                        SingleSelectedDataRow.OnNextParameterValue(default(T));

                        SingleSelectedRowCallback?.Invoke();
                    }
                    break;
                case GridSelectionMode.MultipleSelectedDataRows:
                    if (MultipleSelectedDataRows.SubjectValue != null && MultipleSelectedDataRows.SubjectValue.Count() > 0)
                    {
                        MultipleSelectedDataRows.OnNextParameterValue(MultipleSelectedDataRows.SubjectValue.Intersect(arg));
                        
                        MultipleSelectedRowsCallback?.Invoke();
                    }
                    break;
                default:
                    break;
            }

            return Task.CompletedTask;
        }

        public async Task OnRowSelected(T dataItem)
        {
            //if (ColWidthChangeManager.IsMouseDown)
            //    return;

            switch (SelectionMode.SubjectValue)
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
                        SingleSelectedDataRow.OnNextParameterValue(default(T));
                    }
                    else
                    {
                        SingleSelectedDataRow.OnNextParameterValue(_singleSelectedDataItem);
                    }

                    SingleSelectedRowCallback?.Invoke();

                    break;
                case GridSelectionMode.MultipleSelectedDataRows:
                    if (await _jsService.IsWindowCtrlKeyDown())
                    {
                        if (MultipleSelectedDataRows.SubjectValue == null || MultipleSelectedDataRows.SubjectValue.Count() == 0)//initial selection in the multiple selection mode
                        {
                            MultipleSelectedDataRows.OnNextParameterValue(new [] { dataItem });
                        }
                        else
                        {
                            var tList = MultipleSelectedDataRows.SubjectValue.ToList();
                            if (!tList.Contains(dataItem))
                            {
                                tList.Add(dataItem);
                                MultipleSelectedDataRows.OnNextParameterValue(tList.AsEnumerable());
                            }
                            else
                            {
                                tList.Remove(dataItem);
                                MultipleSelectedDataRows.OnNextParameterValue(tList.AsEnumerable());
                            }
                        }

                        MultipleSelectedRowsCallback?.Invoke();

                    }
                    else if (await _jsService.IsWindowShiftKeyDown())
                    {
                        if (MultipleSelectedDataRows.SubjectValue == null || MultipleSelectedDataRows?.SubjectValue.Count() == 0)//initial selection in the multiple selection mode
                        {
                            MultipleSelectedDataRows.OnNextParameterValue(new[] { dataItem });
                        }
                        else
                        {
                            var tList = MultipleSelectedDataRows.SubjectValue.ToList();

                            var rm = _repositoryService.ViewCollection.SingleOrDefault(x => x.DataItem.Equals(dataItem));//. MultipleSelectedDataRows.OrderBy(dr => dr.)
                            if (rm != null)
                            {
                                var initPos = _repositoryService.ViewCollection.SingleOrDefault(y => y.DataItem.Equals(MultipleSelectedDataRows.SubjectValue.First())).DataItemPosition;

                                int rmMin = initPos;
                                int rmMax = initPos;
                                MultipleSelectedDataRows.SubjectValue.ToList().ForEach(x =>
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
                                MultipleSelectedDataRows.OnNextParameterValue(tList.AsEnumerable());
                            }
                        }

                        MultipleSelectedRowsCallback?.Invoke();
                    }
                    else
                    {
                        MultipleSelectedDataRows.OnNextParameterValue(new[] { dataItem });
                        
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
