using Microsoft.AspNetCore.Components;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public class SmDataGridColumnBase<T>
        : SmDesignComponentBase
    {
        [SmInject]
        public IColumnService ColumnService { get; set; }

        [SmInject]
        public IModelFactoryService ModelFactoryService { get; set; }

        [SmInject]
        public T Model { get; set; }
        //public override async Task SetParametersAsync(ParameterView parameters)
        //{
        //    await base.SetParametersAsync(parameters);

        //    if (Model == null)
        //    {
        //        Model = (T)ModelFactoryService.CreateModel<DataGridCommandColumnModel>(parameters);
        //        ColumnService.RegisterCommandColumn(Model);
        //    }
        //}

    }
}
