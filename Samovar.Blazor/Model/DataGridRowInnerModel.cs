using System.Collections.Generic;

namespace Samovar.Blazor
{
    internal sealed class DataGridRowInnerModel<T>
    {
        public T Data { get; set; }
        public List<DataGridRowCellModel<T>> GridCellModelCollection { get; set; }
    }
}
