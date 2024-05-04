using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public class SmDataGridBase<T>
        : SmDesignComponentBase, IComponentServiceProvider
    {
        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [SmInject]
        public IJsService JsService { get; set; }

        public SmComponentServiceProvider ServiceProvider { get; set; }
        
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        public SmDataGridBase()
        {
            ServiceProvider = new SmComponentServiceProvider();
            ServiceProvider.InitServices<T>();
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender) {
                Lazy<Task<IJSObjectReference>> moduleTask = new(() => JsRuntime.InvokeAsync<IJSObjectReference>(
                   "import", "./_content/Samovar.Blazor/samovar.blazor.js").AsTask());

                JsService.InitJsModule(moduleTask);
            }
            return base.OnAfterRenderAsync(firstRender);
        }
    }
}
