using Microsoft.AspNetCore.Components;
using System.Reactive.Subjects;

namespace Samovar.Grid;

public partial class DataColumnModel
    : DeclarativeColumnModel, IDataColumnModel
{
    public override ColumnType ColumnType { get; } = ColumnType.Data;

    public BehaviorSubject<RenderFragment<object>?> CellShowTemplate { get; } = new BehaviorSubject<RenderFragment<object>?>(null);

    public BehaviorSubject<string> Field { get; } = new BehaviorSubject<string>("");

    public BehaviorSubject<GridTextAlign> TextAlign { get; } = new BehaviorSubject<GridTextAlign>(GridTextAlign.Left);
}
