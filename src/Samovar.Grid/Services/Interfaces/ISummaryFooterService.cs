using System.Reactive.Subjects;

namespace Samovar.Grid;

public interface ISummaryFooterService
{
    IReadOnlyList<SummaryDescriptor> Summaries { get; }
    IReadOnlyList<SummaryDescriptor> GetForField(string field);
    bool HasAny { get; }
    BehaviorSubject<IReadOnlyDictionary<SummaryDescriptor, object?>> Values { get; }
    void RegisterSummary(SummaryDescriptor descriptor);
    void Recalculate();
}
