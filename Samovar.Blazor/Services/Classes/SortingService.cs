using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public class SortingService
        : ISortingService
    {
        public BehaviorSubject<DataGridColumnOrderInfo> ColumnOrderInfo { get; } = new BehaviorSubject<DataGridColumnOrderInfo>(DataGridColumnOrderInfo.Empty);

        string _currentSortingFieldBy = string.Empty;
        bool isAscendingSorting;

        public Task OnColumnClick(IDataColumnModel columnModel)
        {
            DataGridColumnOrderInfo orderInfo = DataGridColumnOrderInfo.Empty;

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
                orderInfo = new DataGridColumnOrderInfo { Field = _currentSortingFieldBy, Asc = isAscendingSorting };

            ColumnOrderInfo.OnNext(orderInfo);

            return Task.CompletedTask;
        }
    }
}
