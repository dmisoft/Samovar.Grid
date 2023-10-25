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

        //private readonly Lazy<IServiceProvider> _serviceProviderLazy;
        public SmComponentServiceProvider ServiceProvider { get; set; }
        
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        public SmDataGridBase()
        {
            ServiceProvider = new SmComponentServiceProvider();
            ServiceProvider.InitServices<T>();
            //_serviceProviderLazy = new Lazy<IServiceProvider>(new Func<IServiceProvider>(ServiceProviderInitializer), LazyThreadSafetyMode.ExecutionAndPublication);
        }
        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender) {
                Lazy<Task<IJSObjectReference>> moduleTask = new(() => JsRuntime.InvokeAsync<IJSObjectReference>(
                   "import", "./_content/Samovar.Blazor/samovar.blazor.js").AsTask());

                JsService.InitJsModule2(moduleTask);
            }
            return base.OnAfterRenderAsync(firstRender);
        }
    }
}
