using Microsoft.AspNetCore.Components;
using System.Reflection;
using System.Linq;

namespace Samovar.Blazor
{
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

        public virtual void InitializeModel(in ParameterView parameters) { }

        public virtual void DependenciesInitialized() { }

        private Task InitializeDependencies(in ParameterView parameters)
        {
            if (!_dependenciesInitialized)
            {
                _dependenciesInitialized = true;

                Dictionary<string, Type> _dict = new Dictionary<string, Type>();
#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
                PropertyInfo[] props = GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields

                foreach (PropertyInfo prop in props)
                {
                    IEnumerable<SmInjectAttribute> attrs = prop.GetCustomAttributes<SmInjectAttribute>(true);
                    foreach (SmInjectAttribute attr in attrs.Where(x => x is not null))
                    {
                        string propName = prop.Name;
                        _dict.Add(propName, prop.PropertyType);
                    }
                }

                IComponentServiceProvider? componentServiceProvider;
                parameters.TryGetValue<IComponentServiceProvider>("ServiceProvider", out componentServiceProvider);

                if (componentServiceProvider != null)
                {
                    foreach (var pair in _dict)
                    {
                        object service = componentServiceProvider.ServiceProvider.GetService(pair.Value);
#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
                        PropertyInfo? piShared = this.GetType().GetProperty(pair.Key, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
                        piShared?.SetValue(this, service);
                    }
                }
                DependenciesInitialized();
            }

            return Task.CompletedTask;
        }
    }
}
