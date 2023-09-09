using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Samovar.Blazor.Filter
{
    public abstract partial class DataGridFilterCellBase<TItem, TFilterCell>
        : SmDesignComponentBase, IDisposable
    {
        [SmInject]
        public IJsService JsService { get; set; }

        [SmInject]
        public ILayoutService LayoutService { get; set; }

        [SmInject]
        public IFilterService FilterService { get; set; }


        [Parameter]
        public IDataColumnModel ColMetadata { get; set; }

        protected string DropdownMenuButtonId { get; } = $"dropdownmenubtn{Guid.NewGuid().ToString().Replace("-", "")}";

        //0 =
        //1 *A*
        //2 A*
        //3 *A
        protected byte _menuMode { get; set; }

        protected DataGridFilterCellInfo FilterCellInfo;// = new DataGridFilterCellInfo();

        private TFilterCell _innerValue = default;

        protected TFilterCell InnerValue
        {
            set
            {
                _innerValue = value;
                
                FilterCellInfo = new DataGridFilterCellInfo { ColumnMetadata = ColMetadata, FilterCellValue = _innerValue, FilterCellMode = _menuMode };
                
                FilterService.Filter(FilterCellInfo);
            }
            get
            {
                return _innerValue;
            }
        }

        protected override void OnInitialized()
        {
            _innerValue = FilterService.TryGetFilterCellValue<TFilterCell>(ColMetadata);
            FilterService.FilterCleared += FilterService_FilterCleared;
        }

        //void ResetFilterCellValue()
        //{
        //    innerValue = default;
        //}

        private Task FilterService_FilterCleared()
        {
            //ClearingMode = true;
            //ResetFilterCellValue();
            _innerValue = default;

            //await InvokeAsync(() => InnerValue = default);
            //ClearingMode = false;
            return Task.CompletedTask;
        }

        //private async Task NotifierService_OnFilterModeChange(byte filterMode, string targetFilterMenuContainerId)
        //{
        //    if (ColMetadata.FilterMenuContainerId == targetFilterMenuContainerId)
        //        await ChangeMode(filterMode);
        //}

        //protected async Task ChangeMode() {
        //    mode = mode+(byte)1 < (byte)4 ? ++mode : (byte)0;
        //    await ChangeMode(mode);
        //}

        //protected async Task ChangeMode(byte newMode)
        //{
        //    mode = newMode;
        //    filterCellInfo.FilterCellMode = mode;
        //    await FilterService.Filter(filterCellInfo);
        //}

        //protected async Task ValueReset() {
        //    MenuMode = 0;
        //    InnerValue = default;
        //    filterCellInfo = new DataGridFilterCellInfo { ColumnMetadata = ColMetadata, FilterCellValue = InnerValue, FilterCellMode = MenuMode };
        //    await FilterService.Filter(filterCellInfo);
        //}

        bool filterMenuOpen = false;
        protected async Task ShowMenu()
        {
            filterMenuOpen = !filterMenuOpen;
            await (await JsService.JsModule()).InvokeVoidAsync("toggleFilterPopupMenu", DropdownMenuButtonId, ColMetadata.FilterMenuContainerId, ColMetadata.FilterMenuId, filterMenuOpen);
        }
        public void Dispose()
        {
            FilterService.FilterCleared -= FilterService_FilterCleared;
        }
    }
}
