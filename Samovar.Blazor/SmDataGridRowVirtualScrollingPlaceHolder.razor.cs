using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace Samovar.Blazor
{
    public partial class SmDataGridRowVirtualScrollingPlaceHolder
        : SmDesignComponentBase, IAsyncDisposable
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [SmInject]
        public IVirtualScrollingNavigationStrategy VirtualScrollingService { get; set; }

        [SmInject]
        public IColumnService ColumnService { get; set; }

        [SmInject]
        public ILayoutService LayoutService { get; set; }

        [SmInject]
        public IGridStateService GridStateService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [Parameter]
        public bool TopPlaceHolder { get; set; }

        protected override Task OnInitializedAsync()
        {
            VirtualScrollingService.VirtualScrollingInfo.Subscribe(VirtualScrollingInfoSubscriber);

            return base.OnInitializedAsync();
        }
        protected string Style { get; set; } = string.Empty;

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

        public ValueTask DisposeAsync()
        {
            return new ValueTask(Task.CompletedTask);
        }
    }
}
