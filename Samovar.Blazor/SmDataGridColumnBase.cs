using Microsoft.AspNetCore.Components;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public class SmDataGridColumnBase<T>
        : SmDesignComponentBase
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [SmInject]
        public IColumnService ColumnService { get; set; }

        [SmInject]
        public IModelFactoryService ModelFactoryService { get; set; }

        [SmInject]
        public T Model { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
