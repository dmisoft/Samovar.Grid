using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public class GridSelectionService<T>
        : IGridSelectionService<T>, IAsyncDisposable
    {
        public BehaviorSubject<GridSelectionMode> SelectionMode { get; } = new BehaviorSubject<GridSelectionMode>(GridSelectionMode.None);

        public BehaviorSubject<T?> SingleSelectedDataRow { get; } = new BehaviorSubject<T?>(default);

        public BehaviorSubject<IEnumerable<T>?> MultipleSelectedDataRows { get; } = new BehaviorSubject<IEnumerable<T>?>(default);

        public Func<Task>? SingleSelectedRowCallback { get; set; }

        public Func<Task>? MultipleSelectedRowsCallback { get; set; }

        T? _singleSelectedDataItem;

        private readonly IJsService _jsService;

        private readonly IRepositoryService<T> _repositoryService;

        public GridSelectionService(IJsService jsService, IRepositoryService<T> repositoryService)
        {
            _jsService = jsService;

            _repositoryService = repositoryService;

            _repositoryService.Data.Subscribe(RepositoryDataSourceChanged);
        }

        private void RepositoryDataSourceChanged(HashSet<T> arg)
        {
            switch (SelectionMode.Value)
            {
                case GridSelectionMode.None:
                    break;
                case GridSelectionMode.Single:
                    if (SingleSelectedDataRow.Value is not null && !arg.Any(x => x is not null && x.Equals(SingleSelectedDataRow.Value)))
                    {
                        SingleSelectedDataRow.OnNext(default);

                        SingleSelectedRowCallback?.Invoke();
                    }
                    break;
                case GridSelectionMode.Multiple:
                    if (MultipleSelectedDataRows.Value != null && MultipleSelectedDataRows.Value.Any())
                    {
                        MultipleSelectedDataRows.OnNext(MultipleSelectedDataRows.Value.Intersect(arg));

                        MultipleSelectedRowsCallback?.Invoke();
                    }
                    break;
                default:
                    break;
            }
        }

        public async Task OnRowSelected(T dataItem)
        {
            switch (SelectionMode.Value)
            {
                case GridSelectionMode.None:
                    break;
                case GridSelectionMode.Single:
                    if (_singleSelectedDataItem is not null && _singleSelectedDataItem.Equals(dataItem))
                    {
                        _singleSelectedDataItem = default;
                    }
                    else
                    {
                        _singleSelectedDataItem = dataItem;
                    }

                    if (_singleSelectedDataItem is null)
                    {
                        SingleSelectedDataRow.OnNext(default);
                    }
                    else
                    {
                        SingleSelectedDataRow.OnNext(_singleSelectedDataItem);
                    }

                    SingleSelectedRowCallback?.Invoke();

                    break;
                case GridSelectionMode.Multiple:
                    if (await _jsService.IsWindowCtrlKeyDown())
                    {
                        if (MultipleSelectedDataRows.Value == null || !MultipleSelectedDataRows.Value.Any())//initial selection in the multiple selection mode
                        {
                            MultipleSelectedDataRows.OnNext(new[] { dataItem });
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
                        if (MultipleSelectedDataRows.Value == null || !MultipleSelectedDataRows.Value.Any())
                        {
                            MultipleSelectedDataRows.OnNext(new[] { dataItem });
                        }
                        else
                        {
                            var tList = MultipleSelectedDataRows.Value.ToList();

                            var rm = _repositoryService.ViewCollection.SingleOrDefault(x => x.DataItem is not null && x.DataItem.Equals(dataItem));//. MultipleSelectedDataRows.OrderBy(dr => dr.)
                            if (rm != null)
                            {
                                var pos = _repositoryService.ViewCollection.Single(y => y.DataItem is not null && y.DataItem.Equals(MultipleSelectedDataRows.Value.First()));
                                
                                int initPos = pos.DataItemPosition;
                                int rmMin = initPos;
                                int rmMax = initPos;
                                
                                MultipleSelectedDataRows.Value.ToList().ForEach(x =>
                                {
                                    var currRm = _repositoryService.ViewCollection.SingleOrDefault(y => y.DataItem is not null && y.DataItem.Equals(x));
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

        public ValueTask DisposeAsync()
        {
            SingleSelectedRowCallback = null;
            MultipleSelectedRowsCallback = null;

            return ValueTask.CompletedTask;
        }
    }
}
