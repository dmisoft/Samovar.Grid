using Microsoft.AspNetCore.Components;

namespace Samovar.Blazor
{
    public class DataColumnModel
        : ColumnModelBase, IDataColumnModel
    {
        public override DataGridColumnType ColumnType { get; } = DataGridColumnType.Data;

        public ISubject<RenderFragment<object>> CellShowTemplate { get; } = new ParameterSubject<RenderFragment<object>>(null);

        public ISubject<RenderFragment<object>> CellEditTemplate { get; } = new ParameterSubject<RenderFragment<object>>(null);

        public ISubject<string> Field { get; } = new ParameterSubject<string>();
    }
}
