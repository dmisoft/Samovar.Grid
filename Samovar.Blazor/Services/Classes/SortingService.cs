using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public class SortingService
        : ISortingService, IDisposable
    {
        public BehaviorSubject<DataGridColumnOrderInfo> ColumnOrderInfo { get; } = new BehaviorSubject<DataGridColumnOrderInfo>(DataGridColumnOrderInfo.Empty);

        string _currenSortingField;
        bool _ascSorting;

        public SortingService()
        {

        }

        public Task OnColumnClick(IDataColumnModel columnModel)
        {
            DataGridColumnOrderInfo orderInfo = DataGridColumnOrderInfo.Empty;

            if (string.IsNullOrEmpty(_currenSortingField)) {//initial setting
                _currenSortingField = columnModel.Field.Value;
                _ascSorting = true;
            }
            else {
                if (_currenSortingField == columnModel.Field.Value)
                {
                    if (!_ascSorting) {//reset to original sorting state of data source
                        _currenSortingField = "";
                        _ascSorting = true;
                    }
                    else {
                        _ascSorting = !_ascSorting;
                    }
                }
                else {//column change
                    _currenSortingField = columnModel.Field.Value;
                    _ascSorting = true;
                }
            }

            if (!string.IsNullOrEmpty(_currenSortingField))
                orderInfo = new DataGridColumnOrderInfo { Field = _currenSortingField, Asc = _ascSorting };

            ColumnOrderInfo.OnNext(orderInfo);


            //RenderFragment<object> fragment = new RenderFragment<object>();
            //fragment.Invoke(null);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}
