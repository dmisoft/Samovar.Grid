using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Samovar.DataGrid
{
    public partial class GridFilterCellChar<TItem>
        : IDisposable
    {
        [CascadingParameter(Name = "datagrid-container")]
        protected SamovarGrid<TItem> DataGrid { get; set; }

        [Parameter]
        public ColumnMetadata ColMetadata { get; set; }

        FilterCellInfo filterCellInfo = new FilterCellInfo();

        private char? innerValue = default;
        protected char? InnerValue
        {
            set 
            {
                if (clearingMode)
                    return;
                innerValue = value.ToString() == "" ? null : value;

                filterCellInfo = new FilterCellInfo { FilterCellValue = innerValue, FilterCellMode = 0 };
                InvokeAsync(async () =>
                {
                    await DataGrid.Filter(ColMetadata, filterCellInfo);
                });
            }
            get
            {
                return innerValue;
            }
        }
        protected override void OnInitialized()
        {
            DataGrid.NotifierService.NotifyOnClearFilter -= NotifierService_NotifyOnClearFilter;
            DataGrid.NotifierService.NotifyOnClearFilter += NotifierService_NotifyOnClearFilter;
        }

        bool clearingMode;
        private async Task NotifierService_NotifyOnClearFilter()
        {
            clearingMode = true;
            await InvokeAsync(() => innerValue = default);
            clearingMode = false;
        }

        protected async Task ValueReset() {
            InnerValue = default;
            filterCellInfo = new FilterCellInfo { FilterCellValue = InnerValue, FilterCellMode = 0 };
            await DataGrid.Filter(ColMetadata, filterCellInfo);
        }

        public void Dispose()
        {
            DataGrid.NotifierService.NotifyOnClearFilter -= NotifierService_NotifyOnClearFilter;
        }
    }
}
