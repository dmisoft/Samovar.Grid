using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Globalization;

namespace Samovar.Blazor.Scrolling
{
    public partial class DataGridScrollbar
        : SmDesignComponentBase, IAsyncDisposable
    {
        private double oldActualTranslateY { get; set; }
        private double actualTranslateY { get; set; }
        private double newScrollbarThumbOffsetY { get; set; }
        protected string Style { get; set; } = string.Empty;


        [Inject]
        public required IJSRuntime jsRuntime { get; set; }


        [Parameter]
        public EventCallback<double> DeltaChangedCallback { set; get; }


        private void HandleMouseDown(MouseEventArgs e)
        {
            oldActualTranslateY = actualTranslateY;
            jsRuntime.InvokeVoidAsync("dataGridScrollbar.handleMouseDown", DotNetObjectReference.Create(this));
        }

        [JSInvokable]
        public void HandleMouseMove(int clientY)
        {
#pragma warning disable S3626 // Jump statements should not be redundant
            return;
#pragma warning restore S3626 // Jump statements should not be redundant
        }

        [JSInvokable]
        public void HandleMouseUp()
        {
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
