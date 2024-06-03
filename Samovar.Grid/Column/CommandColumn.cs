using Microsoft.AspNetCore.Components;

namespace Samovar.Grid;
public class CommandColumn
    : ColumnBase<ICommandColumnModel>
{
    [Parameter]
    public string Width { get; set; } = string.Empty;

    [Parameter]
    public bool? NewButtonVisible { get; set; }

    [Parameter]
    public bool? EditButtonVisible { get; set; }

    [Parameter]
    public bool? DeleteButtonVisible { get; set; }

    public override void DependenciesInitialized()
    {
        ColumnService.RegisterColumn(Model);
    }

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        await base.SetParametersAsync(parameters);

        string? width = parameters.GetValueOrDefault<string>(nameof(Width));
        if (width != null)
            Model.DeclaratedWidthParameter.OnNext(width);

        bool? newButtonVisible = parameters.GetValueOrDefault<bool?>(nameof(NewButtonVisible));
        newButtonVisible ??= true;
        Model.NewButtonVisible.OnNext(newButtonVisible.Value);

        bool? editButtonVisible = parameters.GetValueOrDefault<bool?>(nameof(EditButtonVisible));
        editButtonVisible ??= true;
        Model.EditButtonVisible.OnNext(editButtonVisible.Value);

        bool? deleteButtonVisible = parameters.GetValueOrDefault<bool?>(nameof(DeleteButtonVisible));
        deleteButtonVisible ??= true;
        Model.DeleteButtonVisible.OnNext(deleteButtonVisible.Value);

    }
}
