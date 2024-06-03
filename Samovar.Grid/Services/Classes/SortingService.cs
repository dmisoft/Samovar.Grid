using System.Reactive.Subjects;

namespace Samovar.Grid
{
    public class SortingService
        : ISortingService
    {
        public BehaviorSubject<ColumnOrderInfo> ColumnOrderInfo { get; } = new BehaviorSubject<ColumnOrderInfo>(Grid.ColumnOrderInfo.Empty);

        string _currentSortingFieldBy = string.Empty;
        bool isAscendingSorting;

        public Task OnColumnClick(IDataColumnModel columnModel)
        {
            ColumnOrderInfo orderInfo = Grid.ColumnOrderInfo.Empty;

            if (string.IsNullOrEmpty(_currentSortingFieldBy))
            {
                _currentSortingFieldBy = columnModel.Field.Value;
                isAscendingSorting = true;
            }
            else
            {
                if (_currentSortingFieldBy == columnModel.Field.Value)
                {
                    if (!isAscendingSorting)
                    {
                        _currentSortingFieldBy = "";
                        isAscendingSorting = true;
                    }
                    else
                    {
                        isAscendingSorting = !isAscendingSorting;
                    }
                }
                else
                {
                    _currentSortingFieldBy = columnModel.Field.Value;
                    isAscendingSorting = true;
                }
            }

            if (!string.IsNullOrEmpty(_currentSortingFieldBy))
                orderInfo = new ColumnOrderInfo { Field = _currentSortingFieldBy, Asc = isAscendingSorting };

            ColumnOrderInfo.OnNext(orderInfo);

            return Task.CompletedTask;
        }
    }
}
