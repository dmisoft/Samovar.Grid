using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace Samovar.Blazor;

public class SmDesignComponentBase
    : ComponentBase
{
    [CascadingParameter(Name = "ServiceProvider")]
    IComponentServiceProvider? ServiceProvider { get; set; }

    private bool _dependenciesInitialized;

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        await InitializeDependencies(in parameters);
        await base.SetParametersAsync(parameters);
    }

    public virtual void DependenciesInitialized() { }

    private Task InitializeDependencies(in ParameterView parameters)
    {
        if (!_dependenciesInitialized)
        {
            _dependenciesInitialized = true;

            IComponentServiceProvider? componentServiceProvider = parameters.GetValueOrDefault<IComponentServiceProvider>("ServiceProvider");

            if (componentServiceProvider is not null)
            {
#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
                var propertyInfos = GetType()
                    .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var propertiesWithSmInjectAttribute = propertyInfos.Where(prop =>
                    Attribute.IsDefined(prop, typeof(SmInjectAttribute)));
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields

                propertiesWithSmInjectAttribute.ToList().ForEach(property =>
                {
                    object service = componentServiceProvider.ServiceProvider.GetService(property.PropertyType);
                    property.SetValue(this, service);
                });
            }
            DependenciesInitialized();
        }

        return Task.CompletedTask;
    }
}
