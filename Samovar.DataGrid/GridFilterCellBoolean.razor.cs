using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Samovar.DataGrid
{
    public partial class GridFilterCellBoolean<TItem>
        : IDisposable
    {
        [CascadingParameter(Name = "datagrid-container")]
        protected SamovarGrid<TItem> DataGrid { get; set; }

        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        [Parameter]
        public ColumnMetadata ColMetadata { get; set; }

        protected string DropdownMenuButtonId { get; } = $"dropdownmenu{Guid.NewGuid().ToString().Replace("-", "")}";
        
        protected byte mode = 0;

        FilterCellInfo filterCellInfo = new FilterCellInfo();

        protected bool? InnerValue;
        //protected bool? InnerValue
        //{
        //    set
        //    {
        //        innerValue = value;
                
        //        filterCellInfo = new FilterCellInfo { FilterCellValue = value, FilterCellMode = 0 };
        //        InvokeAsync(async () =>
        //        {
        //            await DataGrid.Filter(ColMetadata, filterCellInfo);
        //        });
        //    }
        //    get
        //    {
        //        return innerValue;
        //    }
        //}
        protected override void OnInitialized()
        {
            DataGrid.NotifierService.NotifyOnFilterModeChange -= NotifierService_OnFilterModeChange;
            DataGrid.NotifierService.NotifyOnFilterModeChange += NotifierService_OnFilterModeChange;
            DataGrid.NotifierService.NotifyOnClearFilter -= NotifierService_NotifyOnClearFilter;
            DataGrid.NotifierService.NotifyOnClearFilter += NotifierService_NotifyOnClearFilter;
        }
        
        //bool clearingMode;
        private async Task NotifierService_NotifyOnClearFilter()
        {
            //clearingMode = true;
            await InvokeAsync(() => InnerValue = null);
            //clearingMode = false;
        }

        private async Task NotifierService_OnFilterModeChange(byte filterMode, string targetFilterMenuContainerId)
        {
            if (ColMetadata.FilterMenuContainerId == targetFilterMenuContainerId)
                await ChangeMode(filterMode);
        }

        protected async Task ChangeMode(byte filterMode)
        {
            mode = filterMode;
            filterCellInfo.FilterCellMode = mode;
            switch (filterMode)
            {
                case 0:
                    filterCellInfo.FilterCellValue = null;
                    InnerValue = null;
                    break;
                case 1:
                    filterCellInfo.FilterCellValue = true;
                    InnerValue = true;
                    break;
                case 2:
                    filterCellInfo.FilterCellValue = false;
                    InnerValue = false;
                    break;
                default:
                    break;
            }
            await DataGrid.Filter(ColMetadata, filterCellInfo);
        }

        protected async Task ValueReset()
        {
            InnerValue = default;
            filterCellInfo = new FilterCellInfo { FilterCellValue = InnerValue, FilterCellMode = 0 };
            await DataGrid.Filter(ColMetadata, filterCellInfo);
        }

        bool filterMenuOpen = false;
        protected async Task ShowMenu()
        {
            filterMenuOpen = !filterMenuOpen;
            await DataGrid.jsModule.InvokeVoidAsync("toggleFilterPopupMenu", DropdownMenuButtonId, ColMetadata.FilterMenuContainerId, ColMetadata.FilterMenuId, filterMenuOpen);
        }

        public void Dispose()
        {
            DataGrid.NotifierService.NotifyOnFilterModeChange -= NotifierService_OnFilterModeChange;
            DataGrid.NotifierService.NotifyOnClearFilter -= NotifierService_NotifyOnClearFilter;
        }
    }
}
