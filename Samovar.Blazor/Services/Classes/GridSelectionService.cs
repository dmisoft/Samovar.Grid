using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public class GridSelectionService<T>
        : IGridSelectionService<T>, IAsyncDisposable
        , IObserver<Task<IEnumerable<GridRowModel<T>>>>
        , IObserver<HashSet<T>>
    {
        private IEnumerable<GridRowModel<T>> ViewCollection = new List<GridRowModel<T>>();

        public BehaviorSubject<RowSelectionMode> SelectionMode { get; } = new BehaviorSubject<RowSelectionMode>(RowSelectionMode.None);

        public BehaviorSubject<T?> SingleSelectedDataRow { get; } = new BehaviorSubject<T?>(default);

        public BehaviorSubject<IEnumerable<T>?> MultipleSelectedDataRows { get; } = new BehaviorSubject<IEnumerable<T>?>(default);

        public Func<Task>? SingleSelectedRowCallback { get; set; }

        public Func<Task>? MultipleSelectedRowsCallback { get; set; }

        T? _singleSelectedDataItem;

        private readonly IJsService _jsService;

        public GridSelectionService(IJsService jsService, IRepositoryService<T> repositoryService)
        {
            _jsService = jsService;
            repositoryService.Data.Subscribe(this);
            repositoryService.ViewCollectionObservableTask.Subscribe(this);
        }

        public async Task OnRowSelected(T dataItem)
        {
            switch (SelectionMode.Value)
            {
                case RowSelectionMode.None:
                    break;
                case RowSelectionMode.Single:
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
                case RowSelectionMode.Multiple:
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

                            var rm = ViewCollection.SingleOrDefault(x => x.DataItem is not null && x.DataItem.Equals(dataItem));//. MultipleSelectedDataRows.OrderBy(dr => dr.)
                            if (rm != null)
                            {
                                var pos = ViewCollection.Single(y => y.DataItem is not null && y.DataItem.Equals(MultipleSelectedDataRows.Value.First()));
                                
                                int initPos = pos.DataItemPosition;
                                int rmMin = initPos;
                                int rmMax = initPos;
                                
                                MultipleSelectedDataRows.Value.ToList().ForEach(x =>
                                {
                                    var currRm = ViewCollection.SingleOrDefault(y => y.DataItem is not null && y.DataItem.Equals(x));
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

                                var tempList = ViewCollection.Where(rm => rm.DataItemPosition >= iFrom && rm.DataItemPosition <= iTo);
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

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public async void OnNext(Task<IEnumerable<GridRowModel<T>>> value)
        {
            ViewCollection = await value;
        }

        public void OnNext(HashSet<T> value)
        {
            switch (SelectionMode.Value)
            {
                case RowSelectionMode.None:
                    break;
                case RowSelectionMode.Single:
                    if (SingleSelectedDataRow.Value is not null && !value.Any(x => x is not null && x.Equals(SingleSelectedDataRow.Value)))
                    {
                        SingleSelectedDataRow.OnNext(default);
                        SingleSelectedRowCallback?.Invoke();
                    }
                    break;
                case RowSelectionMode.Multiple:
                    if (MultipleSelectedDataRows.Value != null && MultipleSelectedDataRows.Value.Any())
                    {
                        MultipleSelectedDataRows.OnNext(MultipleSelectedDataRows.Value.Intersect(value));
                        MultipleSelectedRowsCallback?.Invoke();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
