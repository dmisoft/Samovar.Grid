using Microsoft.AspNetCore.Components;
using System.Reactive.Subjects;

namespace Samovar.Grid;

public interface IDataColumnModel
    : IDeclarativeColumnModel
{
    public BehaviorSubject<RenderFragment<object>?> CellShowTemplate { get; }

    public BehaviorSubject<string> Field { get; }

    public BehaviorSubject<GridTextAlign> TextAlign { get; }
}
