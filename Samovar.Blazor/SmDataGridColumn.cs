using Microsoft.AspNetCore.Components;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public class SmDataGridColumn
        : SmDataGridColumnBase<IDataColumnModel>
    {
        [Parameter]
        public string Field
        {
            get { return null; }
            set
            {
                Model.Field.OnNextParameterValue(value);
            }
        }

        [Parameter]
        public string Title
        {
            get { return null; }
            set
            {
                Model.Title.OnNextParameterValue(value);
            }
        }

        [Parameter]
        public string Width { get { return null; } set { Model.Width.OnNextParameterValue(value); } }

        [Parameter]
        public RenderFragment<object> CellShowTemplate
        {
            get { return null; }
            set { Model.CellShowTemplate.OnNextParameterValue(value); }
        }

        [Parameter]
        public RenderFragment<object> CellEditTemplate { get; set; }

        public override void DependenciesInitialized()
        {
            ColumnService.RegisterColumn(Model);
        }
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);

            //ColumnService.RegisterDataColumn(Model);

            //if (Model == null)
            //{
            //    Model = (IDataColumnModel)ModelFactoryService.CreateModel<DataGridColumnModel>(parameters);
            //    ColumnService.RegisterDataColumn(Model);
            //}
            //if (Model == null)
            //{
            //	if (IsCustomCollectionItem())
            //	{
            //		ModelContainer = parameters.GetValueOrDefault<IModelContainer<TModel>>("ModelContainer");
            //		Model = CreateSettingsModel();
            //		ModelContainer.Register(Model);
            //	}
            //	else
            //	{
            //		ModelOwner = parameters.GetValueOrDefault<IModelProvider<TModel>>("ModelOwner");
            //		Model = ModelOwner.Model();
            //	}
            //	Model.PropertyChanged += OnModelChanged;
            //}
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            return base.OnAfterRenderAsync(firstRender);
        }
    }
}
