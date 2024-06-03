using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace Samovar.Grid
{
    public class ModelFactoryService
        : IModelFactoryService
    {
        public IColumnModel CreateModel<T>(ParameterView parameters)
        {
            PropertyInfo[] interfaceProps = typeof(T).GetProperties();
            var instance = Activator.CreateInstance(typeof(T));
            if (instance == null)
                throw new ArgumentException("");

            IColumnModel retVal = (IColumnModel)instance;

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
