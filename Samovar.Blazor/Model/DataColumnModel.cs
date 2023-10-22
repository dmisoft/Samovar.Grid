using Microsoft.AspNetCore.Components;
using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public class DataColumnModel
        : ColumnModelBase, IDataColumnModel
    {
        public override DataGridColumnType ColumnType { get; } = DataGridColumnType.Data;

        public ISubject<RenderFragment<object>> CellShowTemplate { get; } = new BehaviorSubject<RenderFragment<object>>(null);

        public ISubject<RenderFragment<object>> CellEditTemplate { get; } = new BehaviorSubject<RenderFragment<object>>(null);

        public ISubject<string> Field { get; } = new Subject<string>();
    }
}
