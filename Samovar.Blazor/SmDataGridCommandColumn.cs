using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public class SmDataGridCommandColumn
        : SmDataGridColumnBase<ICommandColumnModel>
    {
        [Parameter]
        public string Width { get { return null; } set { Model.Width.OnNext(value); } }

        [Parameter]
        public bool NewButtonVisible
        {
            get
            {
                return true;
            }
            set
            {
                //Lazy loading
                Model.NewButtonVisible.OnNext(value);
            }
        }

        [Parameter]
        public bool EditButtonVisible
        {
            get
            {
                return true;
            }
            set
            {
                //Lazy loading
                Model.EditButtonVisible.OnNext(value);
            }
        }

        [Parameter]
        public bool DeleteButtonVisible
        {
            get
            {
                return true;
            }
            set
            {
                //Lazy loading
                Model.DeleteButtonVisible.OnNext(value);
            }
        }

        public override void DependenciesInitialized()
        {
            ColumnService.RegisterColumn(Model);
        }
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
        }
    }
}
