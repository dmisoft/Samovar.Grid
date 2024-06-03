using Microsoft.AspNetCore.Components;

namespace Samovar.Grid
{
    public interface IModelFactoryService
    {
        IColumnModel CreateModel<T>(ParameterView parameters);
    }
}
