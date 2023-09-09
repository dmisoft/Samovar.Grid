using System.Collections.Generic;

namespace Samovar.DataGrid
{
    internal sealed class GridRowInnerModel<T>
    {
        public T Data { get; set; }
        public List<GridRowCellModel<T>> GridCellModelCollection { get; set; }
    }
}
