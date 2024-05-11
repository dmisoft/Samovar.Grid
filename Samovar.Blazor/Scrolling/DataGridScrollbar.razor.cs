using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Globalization;

namespace Samovar.Blazor.Scrolling
{
    public partial class DataGridScrollbar
        : SmDesignComponentBase, IAsyncDisposable
    {
        private bool isDragging = false;

        private double startDraggingY;
        private double oldActualTranslateY { get; set; }
        private double actualTranslateY { get; set; }
        private double newScrollbarThumbOffsetY { get; set; }
        protected string Style { get; set; } = string.Empty;


        [Inject]
        public required IJSRuntime jsRuntime { get; set; }

        [SmInject]
        public required IVirtualScrollingNavigationStrategy VirtualScrollingService { get; set; }

        [Parameter]
        public EventCallback<double> DeltaChangedCallback { set; get; }


        private void HandleMouseDown(MouseEventArgs e)
        {
            isDragging = true;
            startDraggingY = e.ClientY;
            oldActualTranslateY = actualTranslateY;
            jsRuntime.InvokeVoidAsync("dataGridScrollbar.handleMouseDown", DotNetObjectReference.Create(this));
        }
        protected override Task OnInitializedAsync()
        {
            VirtualScrollingService.VirtualScrollingInfo.Subscribe(VirtualScrollingInfoSubscriber);
            return base.OnInitializedAsync();
        }

        private void VirtualScrollingInfoSubscriber(DataGridVirtualScrollingInfo info)
        {
            Style = $"height:{info.ScrollContainerHeight.ToString(CultureInfo.InvariantCulture)}px;";
        }

        [JSInvokable]
        public void HandleMouseMove(int clientY)
        {
            double scrollbarThumbHeight = 40;
            var scrollContainerHeight = VirtualScrollingService.VirtualScrollingInfo.Value.ScrollContainerHeight;
            var contentContainerHeight = VirtualScrollingService.VirtualScrollingInfo.Value.ContentContainerHeight;

            if (isDragging)
            {
                var deltaY = clientY - startDraggingY;
                jsRuntime.InvokeVoidAsync("dataGridScrollbar.handleMouseMove", deltaY);

                if (oldActualTranslateY + deltaY < 0)
                {
                    newScrollbarThumbOffsetY = 0;
                }
                else if (oldActualTranslateY + deltaY + scrollbarThumbHeight > scrollContainerHeight)
                {
                    newScrollbarThumbOffsetY = scrollContainerHeight - scrollbarThumbHeight;
                }
                else
                {
                    newScrollbarThumbOffsetY = oldActualTranslateY + deltaY;
                }

                actualTranslateY = newScrollbarThumbOffsetY;

                DeltaChangedCallback.InvokeAsync(actualTranslateY);
                VirtualScrollingService.ScrollTop.OnNext(actualTranslateY * ((contentContainerHeight - scrollContainerHeight) / (scrollContainerHeight - scrollbarThumbHeight)));
            }
        }

        [JSInvokable]
        public void HandleMouseUp()
        {
            isDragging = false;
            actualTranslateY = newScrollbarThumbOffsetY;

            oldActualTranslateY = 0;
            newScrollbarThumbOffsetY = 0;
            jsRuntime.InvokeVoidAsync("dataGridScrollbar.handleMouseUp");
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
