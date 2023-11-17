using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Samovar.Blazor.Scrolling
{
    public partial class DataGridScrollbar
        : SmDesignComponentBase, IAsyncDisposable
    {
        private bool isDragging = false;
        private double startDraggingY;
        private double actualTranslateY { get; set; }
        private double newOffsetY { get; set; }
        
        [Inject] IJSRuntime jsRuntime { get; set; }

        [SmInject]
        public IVirtualScrollingNavigationStrategy VirtualScrollingService { get; set; }

        [Parameter]
        public EventCallback<double> DeltaChangedCallback { set; get; }

        private void HandleMouseDown(MouseEventArgs e)
        {
            isDragging = true;
            startDraggingY = e.ClientY;

            // Subscribe to global mousemove and mouseup events using JavaScript Interop
            jsRuntime.InvokeVoidAsync("dataGridScrollbar.handleMouseDown", DotNetObjectReference.Create(this));
        }
        protected override Task OnInitializedAsync()
        {
            VirtualScrollingService.VirtualScrollingInfo.Subscribe(VirtualScrollingInfoSubscriber);
            return base.OnInitializedAsync();
        }

        private void VirtualScrollingInfoSubscriber(DataGridVirtualScrollingInfo info)
        {
            //throw new NotImplementedException();
        }

        [JSInvokable]
        public void HandleMouseMove(int clientY)
        {
            if (isDragging)
            {
                var deltaY = clientY - startDraggingY;
                jsRuntime.InvokeVoidAsync("dataGridScrollbar.handleMouseMove", deltaY);

                if (actualTranslateY + deltaY < 0)
                {
                    newOffsetY = 0;
                }
                else if (actualTranslateY + deltaY + 40 > VirtualScrollingService.VirtualScrollingInfo.Value.ScrollContainerHeight)
                {
                    newOffsetY = VirtualScrollingService.VirtualScrollingInfo.Value.ScrollContainerHeight - 40;
                }
                else
                {
                    newOffsetY = actualTranslateY + deltaY;
                }
                DeltaChangedCallback.InvokeAsync(newOffsetY);
                var scrollContainerHeight = VirtualScrollingService.VirtualScrollingInfo.Value.ScrollContainerHeight;
                var contentContainerHeight = VirtualScrollingService.VirtualScrollingInfo.Value.ContentContainerHeight;
                VirtualScrollingService.ScrollTop.OnNext(newOffsetY * (contentContainerHeight / scrollContainerHeight));
                //StateHasChanged();
            }
        }

        [JSInvokable]
        public void HandleMouseUp()
        {
            isDragging = false;
            actualTranslateY = newOffsetY;

            // Unsubscribe from global mousemove and mouseup events using JavaScript Interop
            jsRuntime.InvokeVoidAsync("dataGridScrollbar.handleMouseUp");
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
            //throw new NotImplementedException();
        }
    }
}
