using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Samovar.Blazor;

public class GridSelectionService<T>
    : IGridSelectionService<T>, IAsyncDisposable
    , IObserver<Task<IEnumerable<GridRowModel<T>>>>
    , IObserver<IEnumerable<T>>
    , IObserver<RowSelectionMode>
{
    private IEnumerable<GridRowModel<T>> ViewCollection = new List<GridRowModel<T>>();

    public BehaviorSubject<RowSelectionMode> SelectionMode { get; } = new BehaviorSubject<RowSelectionMode>(RowSelectionMode.None);

    public BehaviorSubject<T?> SingleSelectedDataRow { get; private set; } = new BehaviorSubject<T?>(default);

    public BehaviorSubject<IEnumerable<T>?> MultipleSelectedDataRows { get; private set; } = new BehaviorSubject<IEnumerable<T>?>(default);

    public Func<Task>? SingleSelectedRowCallback { get; set; }

    public Func<Task>? MultipleSelectedRowsCallback { get; set; }

    T? _singleSelectedDataItem;

    private readonly IJsService _jsService;

    public GridSelectionService(IJsService jsService, IRepositoryService<T> repositoryService, IDataSourceService<T> dataSourceService)
    {
        _jsService = jsService;
        dataSourceService.Data.DistinctUntilChanged().Subscribe(this);
        repositoryService.ViewCollectionObservableTask.Subscribe(this);
        SelectionMode.DistinctUntilChanged().Subscribe(this);

        SingleSelectedDataRow.DistinctUntilChanged().Subscribe(SingleSelectedDataRow =>
        {
            SingleSelectedRowCallback?.Invoke();
        });

        MultipleSelectedDataRows.DistinctUntilChanged().Subscribe(MultipleSelectedDataRows =>
        {
            MultipleSelectedRowsCallback?.Invoke();
        });
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
                        MultipleSelectedDataRows.OnNext([dataItem]);
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
                        MultipleSelectedDataRows.OnNext([dataItem]);
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
                    MultipleSelectedDataRows.OnNext([dataItem]);
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

    public void OnNext(RowSelectionMode value)
    {
        Reset();
    }

    public void OnNext(IEnumerable<T> value)
    {
        if (SingleSelectedDataRow.Value is not null && !value.Any(x => x is not null && x.Equals(SingleSelectedDataRow.Value)))
        {
            SingleSelectedDataRow.OnNext(default);
            SingleSelectedRowCallback?.Invoke();
        }

        if (MultipleSelectedDataRows.Value is not null)
        {
            var intersectionList = MultipleSelectedDataRows.Value.Intersect(value);
            MultipleSelectedDataRows.OnNext(intersectionList);
            MultipleSelectedRowsCallback?.Invoke();
        }
    }

    private void Reset()
    {
        SingleSelectedDataRow.OnCompleted();
        SingleSelectedDataRow = new BehaviorSubject<T?>(default);

        MultipleSelectedDataRows.OnCompleted();
        MultipleSelectedDataRows = new BehaviorSubject<IEnumerable<T>?>(default);

        SingleSelectedRowCallback?.Invoke();
        MultipleSelectedRowsCallback?.Invoke();
    }
}
