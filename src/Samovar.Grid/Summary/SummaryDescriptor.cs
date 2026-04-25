namespace Samovar.Grid;

public sealed class SummaryDescriptor
{
    public required string Field { get; init; }
    public GridSummaryAggregate Aggregate { get; init; } = GridSummaryAggregate.None;
    public string? Label { get; init; }
    public string? Format { get; init; }
}
