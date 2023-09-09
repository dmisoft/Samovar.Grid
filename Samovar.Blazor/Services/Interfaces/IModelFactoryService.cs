using Microsoft.AspNetCore.Components;

namespace Samovar.Blazor
{
    public interface IModelFactoryService
    {
        IColumnModel CreateModel<T>(ParameterView parameters);
    }
}
