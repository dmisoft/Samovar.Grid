using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Globalization;
using System.Threading.Tasks;

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
		protected string Style { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [Inject] IJSRuntime jsRuntime { get; set; }

        [SmInject]
        public IVirtualScrollingNavigationStrategy VirtualScrollingService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [Parameter]
        public EventCallback<double> DeltaChangedCallback { set; get; }


        private void HandleMouseDown(MouseEventArgs e)
        {
            isDragging = true;
            startDraggingY = e.ClientY;
            oldActualTranslateY = actualTranslateY;
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
                VirtualScrollingService.ScrollTop.OnNext(actualTranslateY * ((contentContainerHeight - scrollContainerHeight) / (scrollContainerHeight- scrollbarThumbHeight)));
                //StateHasChanged();
            }
        }

        [JSInvokable]
        public void HandleMouseUp()
        {
            isDragging = false;
            actualTranslateY = newScrollbarThumbOffsetY;
			
            oldActualTranslateY = 0;
            newScrollbarThumbOffsetY = 0;
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
