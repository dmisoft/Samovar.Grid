using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace Samovar.Grid;

public partial class GridSummaryCell
    : DesignComponentBase, IAsyncDisposable
{
    [Parameter]
    public required IDataColumnModel ColumnModel { get; set; }

    [SmInject]
    public required ISummaryFooterService SummaryFooterService { get; set; }

    protected string WidthStyle = "";
    protected string TextAlignStyle = "";

    protected IReadOnlyList<SummaryDescriptor> Descriptors { get; private set; } = [];

    private IDisposable? _widthSub;
    private IDisposable? _fieldSub;
    private IDisposable? _textAlignSub;
    private IDisposable? _valuesSub;
    private IDisposable? _formatSub;

    protected override Task OnInitializedAsync()
    {
        _widthSub = ColumnModel.WidthStyle.Subscribe(w => { WidthStyle = w; _ = InvokeAsync(StateHasChanged); });
        _fieldSub = ColumnModel.Field.Subscribe(_ => RefreshDescriptors());
        _textAlignSub = ColumnModel.TextAlign.Subscribe(align =>
        {
            TextAlignStyle = align switch
            {
                GridTextAlign.Center => "justify-content:center;",
                GridTextAlign.Right => "justify-content:flex-end;",
                _ => "justify-content:flex-start;"
            };
            _ = InvokeAsync(StateHasChanged);
        });
        _formatSub = ColumnModel.Format.Subscribe(_ => InvokeAsync(StateHasChanged));
        _valuesSub = SummaryFooterService.Values.Subscribe(_ => InvokeAsync(StateHasChanged));
        return base.OnInitializedAsync();
    }

    private void RefreshDescriptors()
    {
        Descriptors = SummaryFooterService.GetForField(ColumnModel.Field.Value);
        _ = InvokeAsync(StateHasChanged);
    }

    protected string LabelFor(SummaryDescriptor descriptor)
    {
        if (!string.IsNullOrEmpty(descriptor.Label))
            return descriptor.Label;

        return descriptor.Aggregate switch
        {
            GridSummaryAggregate.Sum => L10n[LocalizationKeys.SummarySum],
            GridSummaryAggregate.Average => L10n[LocalizationKeys.SummaryAverage],
            GridSummaryAggregate.Count => L10n[LocalizationKeys.SummaryCount],
            GridSummaryAggregate.Min => L10n[LocalizationKeys.SummaryMin],
            GridSummaryAggregate.Max => L10n[LocalizationKeys.SummaryMax],
            _ => string.Empty
        };
    }

    protected string FormatValue(SummaryDescriptor descriptor)
    {
        var values = SummaryFooterService.Values.Value;
        if (!values.TryGetValue(descriptor, out var value) || value is null)
            return "-";

        var format = descriptor.Format ?? ColumnModel.Format.Value;

        if (value is IFormattable formattable)
        {
            try
            {
                return formattable.ToString(format, CultureInfo.CurrentCulture);
            }
            catch (FormatException)
            {
                return value.ToString() ?? string.Empty;
            }
        }

        return value.ToString() ?? string.Empty;
    }

    public ValueTask DisposeAsync()
    {
        _widthSub?.Dispose();
        _fieldSub?.Dispose();
        _textAlignSub?.Dispose();
        _formatSub?.Dispose();
        _valuesSub?.Dispose();
        return ValueTask.CompletedTask;
    }
}
