using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public class SmDesignComponentBase
        : ComponentBase 
    {
        [CascadingParameter(Name = "ServiceProvider")]
        IComponentServiceProvider ServiceProvider { get; set; }

        private bool _dependenciesInitialized;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
        }

        public override Task SetParametersAsync(ParameterView parameters)
        {
            InitializeDependencies(in parameters);
            return base.SetParametersAsync(parameters);
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

                try
                {
                    foreach (PropertyInfo prop in props)
                    {
                        object[] attrs = prop.GetCustomAttributes(true);
                        foreach (object attr in attrs)
                        {
                            SmInjectAttribute injectAttr = attr as SmInjectAttribute;
                            if (injectAttr != null)
                            {
                                string propName = prop.Name;
                                _dict.Add(propName, prop.PropertyType);
                            }
                        }
                    }
                }
                catch
                {
                    throw;
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
                        PropertyInfo piShared = this.GetType().GetProperty(pair.Key, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        piShared.SetValue(this, service);
                    }
                }
                DependenciesInitialized();
            }

            return Task.CompletedTask;
        }
    }
}
