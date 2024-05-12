using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace Samovar.Blazor
{
    public class ModelFactoryService
        : IModelFactoryService
    {
        public IColumnModel CreateModel<T>(ParameterView parameters)
        {
            PropertyInfo[] interfaceProps = typeof(T).GetProperties();
            IColumnModel retVal = (IColumnModel)Activator.CreateInstance(typeof(T));

            foreach (PropertyInfo prop in interfaceProps)
            {
                if (parameters.TryGetValue<dynamic>(prop.Name, out var value))
                {
                    prop.SetValue(retVal, value);
                }
            }


            return retVal;
        }
    }
}
