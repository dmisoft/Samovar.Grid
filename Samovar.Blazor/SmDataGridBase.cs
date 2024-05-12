using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Reflection;

namespace Samovar.Blazor
{
    public class SmDataGridBase<T>
        : ComponentBase, IComponentServiceProvider
    {
        [Inject]
        public required IJSRuntime JsRuntime { get; set; } = default!;

        [SmInject]
        public required IJsService JsService { get; set; } = default!;

        public SmComponentServiceProvider ServiceProvider { get; }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        public SmDataGridBase()
        {
            ServiceProvider = new SmComponentServiceProvider();
            ServiceProvider.InitServices<T>();
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await InitializeDependencies();
            await base.SetParametersAsync(parameters);
        }
        private bool _dependenciesInitialized;

        private Task InitializeDependencies()
        {
            if (!_dependenciesInitialized)
            {
                _dependenciesInitialized = true;

                Dictionary<string, Type> _dict = new Dictionary<string, Type>();

#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
                PropertyInfo[] props = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields

                foreach (PropertyInfo prop in props)
                {
                    IEnumerable<SmInjectAttribute> attrs = prop.GetCustomAttributes<SmInjectAttribute>(true);
                    foreach (SmInjectAttribute attr in attrs)
                    {
                        if (attr != null)
                        {
                            string propName = prop.Name;
                            _dict.Add(propName, prop.PropertyType);
                        }
                    }
                }

                foreach (var pair in _dict)
                {
                    object service = ServiceProvider.GetService(pair.Value);
                    PropertyInfo? piShared = this.GetType().GetProperty(pair.Key, BindingFlags.Public | BindingFlags.Instance);
                    piShared?.SetValue(this, service);
                }
            }

            return Task.CompletedTask;
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Lazy<Task<IJSObjectReference>> moduleTask = new(() => JsRuntime.InvokeAsync<IJSObjectReference>(
                   "import", "./_content/Samovar.Blazor/samovar.blazor.js").AsTask());

                JsService.InitJsModule(moduleTask);
            }

            return base.OnAfterRenderAsync(firstRender);
        }
    }
}
