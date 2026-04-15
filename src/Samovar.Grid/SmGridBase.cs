using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Reflection;

namespace Samovar.Grid;

public class SmGridBase<T>
    : ComponentBase, IComponentServiceProvider
{
    [Inject]
    public required IJSRuntime JsRuntime { get; set; } = default!;

    [SmInject]
    public required IJsService JsService { get; set; } = default!;

    public SmComponentServiceProvider ServiceProvider { get; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    public SmGridBase()
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

#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
            var propertyInfos = GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var propertiesWithSmInjectAttribute = propertyInfos.Where(prop =>
                Attribute.IsDefined(prop, typeof(SmInjectAttribute)));
#pragma warning restore S3011

            propertiesWithSmInjectAttribute.ToList().ForEach(property =>
            {
                object service = ServiceProvider.GetService(property.PropertyType);
                property.SetValue(this, service);
            });
        }

        return Task.CompletedTask;
    }
}
