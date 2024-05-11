using Microsoft.AspNetCore.Components;
using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public class DataColumnModel
        : ColumnModelBase, IDataColumnModel
    {
        public override DataGridColumnType ColumnType { get; } = DataGridColumnType.Data;

        public BehaviorSubject<RenderFragment<object>?> CellShowTemplate { get; } = new BehaviorSubject<RenderFragment<object>?>(null);

        public BehaviorSubject<RenderFragment<object>?> CellEditTemplate { get; } = new BehaviorSubject<RenderFragment<object>?>(null);

        public BehaviorSubject<string> Field { get; } = new BehaviorSubject<string>("");
    }
}
