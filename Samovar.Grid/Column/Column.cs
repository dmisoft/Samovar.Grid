﻿using Microsoft.AspNetCore.Components;

namespace Samovar.Grid;
public class Column
    : ColumnBase<IDataColumnModel>
{
    [Parameter]
    public required string Field { get; set; }

    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public string? Width { get; set; }

    [Parameter]
    public RenderFragment<object>? CellShowTemplate { get; set; }

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
        title??=field;
        Model.Title.OnNext(title);

        var width = parameters.GetValueOrDefault<string>(nameof(Width));
        if (width is not null)
            Model.DeclaratedWidthParameter.OnNext(width);

        var cellShowTemplate = parameters.GetValueOrDefault<RenderFragment<object>>(nameof(CellShowTemplate));
        if (cellShowTemplate is not null)
            Model.CellShowTemplate.OnNext(cellShowTemplate);
    }
}
