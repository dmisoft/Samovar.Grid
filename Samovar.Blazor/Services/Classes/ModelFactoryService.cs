using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Samovar.Blazor
{
    public class ModelFactoryService
        : IModelFactoryService, IDisposable
    {
        public IColumnModel CreateModel<T>(ParameterView parameters)
        {
            Dictionary<string, Type> _dict = new Dictionary<string, Type>();

            PropertyInfo[] interfaceProps = typeof(T).GetProperties();// BindingFlags.Public | BindingFlags.NonPublic);
            IColumnModel retVal = (IColumnModel)Activator.CreateInstance(typeof(T));

            foreach (PropertyInfo prop in interfaceProps)
            {
                if (parameters.TryGetValue<dynamic>(prop.Name, out var value))
                {
                    //var v = parameters.GetValueOrDefault<string>(prop.Name);
                    //if (!string.IsNullOrEmpty(value))
                    {
                        prop.SetValue(retVal, value);
                    }
                }
            }

            //IComponentServiceProvider componentServiceProvider;

            //if (this is IComponentServiceProvider)
            //    componentServiceProvider = this as IComponentServiceProvider;
            //else
            //    componentServiceProvider = parameters.GetValueOrDefault<IComponentServiceProvider>("ServiceProvider");

            //if (componentServiceProvider != null)
            //{
            //    foreach (var pair in _dict)
            //    {
            //        object service = componentServiceProvider.ServiceProvider.GetService(pair.Value);
            //        PropertyInfo piShared = this.GetType().GetProperty(pair.Key, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            //        piShared.SetValue(this, service);
            //    }
            //}


            //if (parameters.TryGetValue<string>(nameof(Param), out var value))
            //{
            //    if (value is null)
            //    {
            //        message = "The value of 'Param' is null.";
            //    }
            //    else
            //    {
            //        message = $"The value of 'Param' is {value}.";
            //    }
            //}

            return retVal;
        }

        public void Dispose()
        {

        }
    }
}
