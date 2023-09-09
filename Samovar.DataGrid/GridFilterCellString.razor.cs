using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Samovar.DataGrid
{
    public partial class GridFilterCellString<TItem>
        : IDisposable
    {
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        [CascadingParameter(Name = "datagrid-container")]
        protected SamovarGrid<TItem> DataGrid { get; set; }

        [Parameter]
        public ColumnMetadata ColMetadata { get; set; }
        protected string DropdownMenuButtonId { get; } = $"dropdownmenubtn{Guid.NewGuid().ToString().Replace("-", "")}";
        //protected string MenuId { get; } = Guid.NewGuid().ToString();

        //0 =
        //1 *A*
        //2 A*
        //3 *A
        protected byte mode = 0;

        FilterCellInfo filterCellInfo = new FilterCellInfo();

        private string innerValue = default;
        protected string InnerValue
        {
            set 
            {
                if (clearingMode)
                    return;
                innerValue = string.IsNullOrEmpty(value) ? null : value;

                filterCellInfo = new FilterCellInfo { FilterCellValue = innerValue, FilterCellMode = mode };
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
            DataGrid.NotifierService.NotifyOnFilterModeChange -= NotifierService_OnFilterModeChange;
            DataGrid.NotifierService.NotifyOnFilterModeChange += NotifierService_OnFilterModeChange;
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

        private async Task NotifierService_OnFilterModeChange(byte filterMode, string targetFilterMenuContainerId)
        {
            if (ColMetadata.FilterMenuContainerId == targetFilterMenuContainerId)
                await ChangeMode(filterMode);
        }

        protected async Task ChangeMode() {
            mode = mode+(byte)1 < (byte)4 ? ++mode : (byte)0;
            await ChangeMode(mode);
        }

        protected async Task ChangeMode(byte newMode)
        {
            mode = newMode;
            filterCellInfo.FilterCellMode = mode;
            await DataGrid.Filter(ColMetadata, filterCellInfo);
        }

        protected async Task ValueReset() {
            mode = 0;
            InnerValue = default;
            filterCellInfo = new FilterCellInfo { FilterCellValue = InnerValue, FilterCellMode = mode };
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
