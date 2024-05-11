using Microsoft.AspNetCore.Components;

namespace Samovar.Blazor
{
    public class SmDataGridColumn
        : SmDataGridColumnBase<IDataColumnModel>
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [Parameter]
        public string Field { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string Width { get; set; }

        [Parameter]
        public RenderFragment<object> CellShowTemplate { get; set; }

        [Parameter]
        public RenderFragment<object> CellEditTemplate { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public override void DependenciesInitialized()
        {
            ColumnService.RegisterColumn(Model);
        }
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);

            var field = parameters.GetValueOrDefault<string>("Field");
            if (field is not null)
                Model.Field.OnNext(field);

            var title = parameters.GetValueOrDefault<string>("Title");
            if (title is not null)
                Model.Title.OnNext(title);

            var width = parameters.GetValueOrDefault<string>("Width");
            if (width is not null)
                Model.Width.OnNext(width);

            var cellShowTemplate = parameters.GetValueOrDefault<RenderFragment<object>>("CellShowTemplate");
            if (cellShowTemplate is not null)
                Model.CellShowTemplate.OnNext(cellShowTemplate);

            var cellEditTemplate = parameters.GetValueOrDefault<RenderFragment<object>>("CellEditTemplate");
            if (cellEditTemplate is not null)
                Model.CellEditTemplate.OnNext(cellEditTemplate);
        }
    }
}
