using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public class SortingService
        : ISortingService
    {
        public BehaviorSubject<DataGridColumnOrderInfo> ColumnOrderInfo { get; } = new BehaviorSubject<DataGridColumnOrderInfo>(DataGridColumnOrderInfo.Empty);

        string _currenSortingField = string.Empty;
        bool _ascSorting;

        public Task OnColumnClick(IDataColumnModel columnModel)
        {
            DataGridColumnOrderInfo orderInfo = DataGridColumnOrderInfo.Empty;

            if (string.IsNullOrEmpty(_currenSortingField))
            {
                _currenSortingField = columnModel.Field.Value;
                _ascSorting = true;
            }
            else
            {
                if (_currenSortingField == columnModel.Field.Value)
                {
                    if (!_ascSorting)
                    {
                        _currenSortingField = "";
                        _ascSorting = true;
                    }
                    else
                    {
                        _ascSorting = !_ascSorting;
                    }
                }
                else
                {
                    _currenSortingField = columnModel.Field.Value;
                    _ascSorting = true;
                }
            }

            if (!string.IsNullOrEmpty(_currenSortingField))
                orderInfo = new DataGridColumnOrderInfo { Field = _currenSortingField, Asc = _ascSorting };

            ColumnOrderInfo.OnNext(orderInfo);

            return Task.CompletedTask;
        }
    }
}
