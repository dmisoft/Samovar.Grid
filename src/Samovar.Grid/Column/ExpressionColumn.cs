using Microsoft.AspNetCore.Components;

namespace Samovar.Grid;

public class ExpressionColumn
    : ColumnBase<IExpressionColumnModel>
{
    [Parameter]
    public required string Formula { get; set; }

    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public string? Width { get; set; }

    [Parameter]
    public bool Resizable { get; set; } = true;

    [Parameter]
    public double MinWidth { get; set; } = 50d;

    [Parameter]
    public GridTextAlign TextAlign { get; set; } = GridTextAlign.Right;

    [Parameter]
    public string? Format { get; set; }

    public override void DependenciesInitialized()
    {
        ColumnService.RegisterColumn(Model);
    }

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        await base.SetParametersAsync(parameters);

        var formula = parameters.GetValueOrDefault<string>(nameof(Formula));
        if (formula is null)
            throw new InvalidOperationException("ExpressionColumn requires a Formula parameter.");
        Model.Formula.OnNext(formula);

        var title = parameters.GetValueOrDefault<string>(nameof(Title)) ?? string.Empty;
        Model.Title.OnNext(title);

        var width = parameters.GetValueOrDefault<string>(nameof(Width));
        if (width is not null)
            Model.DeclaratedWidthParameter.OnNext(width);

        Model.Resizable = Resizable;
        Model.MinWidth = MinWidth;

        Model.TextAlign.OnNext(TextAlign);

        var format = parameters.GetValueOrDefault<string>(nameof(Format));
        Model.Format.OnNext(format);
    }
}
