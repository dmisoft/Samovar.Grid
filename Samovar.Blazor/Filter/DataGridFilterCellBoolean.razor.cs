using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Samovar.Blazor.Filter
{
    public partial class DataGridFilterCellBoolean<TItem>
        : DataGridFilterCellBase<TItem, bool?> ,IDisposable
    {
        //protected async Task ChangeMode(byte filterMode)
        //{
        //    mode = filterMode;
        //    filterCellInfo.FilterCellMode = mode;
        //    switch (filterMode)
        //    {
        //        case 0:
        //            filterCellInfo.FilterCellValue = null;
        //            InnerValue = null;
        //            break;
        //        case 1:
        //            filterCellInfo.FilterCellValue = true;
        //            InnerValue = true;
        //            break;
        //        case 2:
        //            filterCellInfo.FilterCellValue = false;
        //            InnerValue = false;
        //            break;
        //        default:
        //            break;
        //    }
        //    await DataGrid.Filter(ColMetadata, filterCellInfo);
        //}
    }
}
