

using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public class SortingService
        : ISortingService, IDisposable
    {
        public ISubject<DataGridColumnOrderInfo> ColumnOrderInfo { get; } = new ParameterSubject<DataGridColumnOrderInfo>(DataGridColumnOrderInfo.Empty);

        string _currenSortingField;
        bool _ascSorting;

        public SortingService()
        {

        }

        public Task OnColumnClick(IDataColumnModel columnModel)
        {
            DataGridColumnOrderInfo orderInfo = DataGridColumnOrderInfo.Empty;

            if (string.IsNullOrEmpty(_currenSortingField)) {//initial setting
                _currenSortingField = columnModel.Field.SubjectValue;
                _ascSorting = true;
            }
            else {
                if (_currenSortingField == columnModel.Field.SubjectValue)
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
                    _currenSortingField = columnModel.Field.SubjectValue;
                    _ascSorting = true;
                }
            }

            if (!string.IsNullOrEmpty(_currenSortingField))
                orderInfo = new DataGridColumnOrderInfo { Field = _currenSortingField, Asc = _ascSorting };

            ColumnOrderInfo.OnNextParameterValue(orderInfo);


            //RenderFragment<object> fragment = new RenderFragment<object>();
            //fragment.Invoke(null);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}
