using Microsoft.AspNetCore.Components;

namespace Samovar.Grid;

public class SummaryItem
    : DesignComponentBase
{
    [SmInject]
    public required ISummaryFooterService SummaryFooterService { get; set; }

    [Parameter, EditorRequired]
    public required string Field { get; set; }

    [Parameter]
    public GridSummaryAggregate Aggregate { get; set; } = GridSummaryAggregate.None;

    [Parameter]
    public string? Label { get; set; }

    [Parameter]
    public string? Format { get; set; }

    private bool _registered;

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        await base.SetParametersAsync(parameters);

        if (_registered)
            return;
        _registered = true;

        SummaryFooterService.RegisterSummary(new SummaryDescriptor
        {
            Field = Field,
            Aggregate = Aggregate,
            Label = Label,
            Format = Format
        });
    }
}
