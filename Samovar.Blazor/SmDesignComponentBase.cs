using Microsoft.AspNetCore.Components;
using System.Reflection;

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

                PropertyInfo[] props = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

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

                IComponentServiceProvider componentServiceProvider;

                if (this is IComponentServiceProvider)
                    componentServiceProvider = this as IComponentServiceProvider;
                else
                    componentServiceProvider = parameters.GetValueOrDefault<IComponentServiceProvider>("ServiceProvider");

                if (componentServiceProvider != null)
                {
                    foreach (var pair in _dict)
                    {
                        object service = componentServiceProvider.ServiceProvider.GetService(pair.Value);
                        PropertyInfo? piShared = this.GetType().GetProperty(pair.Key, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        piShared?.SetValue(this, service);
                    }
                }
                DependenciesInitialized();
            }

            return Task.CompletedTask;
        }
    }
}
