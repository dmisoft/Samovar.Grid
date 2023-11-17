using Microsoft.AspNetCore.Components;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public partial class SmDataGridRowVirtualScrollingPlaceHolder
        : SmDesignComponentBase, IDisposable
    {
        //[CascadingParameter(Name = "datagrid-row")]
        //protected SmDataGridRow<T> GridRow { get; set; }
        [SmInject]
        public IVirtualScrollingNavigationStrategy VirtualScrollingService { get; set; }

        [SmInject]
        public IColumnService ColumnService { get; set; }

        [SmInject]
        public ILayoutService LayoutService { get; set; }

        [SmInject]
        public IGridStateService GridStateService { get; set; }

        [Parameter]
        public bool TopPlaceHolder { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
        }

        public override Task SetParametersAsync(ParameterView parameters)
        {
            return base.SetParametersAsync(parameters);
        }

        public void Dispose()
        {
            GC.Collect();
        }

        protected override Task OnInitializedAsync()
        {
            VirtualScrollingService.VirtualScrollingInfo.Subscribe(VirtualScrollingInfoSubscriber);

            return base.OnInitializedAsync();
        }
        protected string Style { get; set; }
        private void VirtualScrollingInfoSubscriber(DataGridVirtualScrollingInfo info)
        {
            if (TopPlaceHolder)
            {
                Style = $"cursor:pointer;height:{info.TopPlaceholderHeight.ToString(CultureInfo.InvariantCulture)}px;";
            }
            else
            {
                Style = $"cursor:pointer;height:{info.BottomPlaceholderHeight.ToString(CultureInfo.InvariantCulture)}px;";
            }
            StateHasChanged();
        }
    }
}
