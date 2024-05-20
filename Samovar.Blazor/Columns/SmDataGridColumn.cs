using Microsoft.AspNetCore.Components;

namespace Samovar.Blazor;
public class SmDataGridColumn
    : SmDataGridColumnBase<IDataColumnModel>
{
    [Parameter]
    public required string Field { get; set; }

    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public string? Width { get; set; }

    [Parameter]
    public RenderFragment<object>? CellShowTemplate { get; set; }

    [Parameter]
    public RenderFragment<object>? CellEditTemplate { get; set; }

    public override void DependenciesInitialized()
    {
        ColumnService.RegisterColumn(Model);
    }
    public override async Task SetParametersAsync(ParameterView parameters)
    {
        await base.SetParametersAsync(parameters);

        var field = parameters.GetValueOrDefault<string>(nameof(Field));
        
        if (field is null)
            throw new InvalidOperationException("");
        Model.Field.OnNext(field);

        var title = parameters.GetValueOrDefault<string>(nameof(Title));
        title??=nameof(Model.Field);
        Model.Title.OnNext(title);

        var width = parameters.GetValueOrDefault<string>(nameof(Width));
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
