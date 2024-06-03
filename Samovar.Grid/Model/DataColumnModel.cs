using Microsoft.AspNetCore.Components;
using System.Reactive.Subjects;
using System.Reflection;

namespace Samovar.Grid;

public partial class DataColumnModel
	: DeclarativeColumnModel, IDataColumnModel
{
	public override ColumnType ColumnType { get; } = ColumnType.Data;

	public BehaviorSubject<RenderFragment<object>?> CellShowTemplate { get; } = new BehaviorSubject<RenderFragment<object>?>(null);

	public BehaviorSubject<RenderFragment<object>?> CellEditTemplate { get; } = new BehaviorSubject<RenderFragment<object>?>(null);

	public BehaviorSubject<string> Field { get; } = new BehaviorSubject<string>("");

	public PropertyInfo ColumnDataItemPropertyInfo => throw new NotImplementedException();

	public bool? SortingAscending { get; set; }

}
