using Microsoft.AspNetCore.Components;
using System.Reactive.Subjects;
using System.Reflection;

namespace Samovar.Grid;

public interface IDataColumnModel
	: IDeclarativeColumnModel
{
	PropertyInfo ColumnDataItemPropertyInfo { get; }

	public BehaviorSubject<RenderFragment<object>?> CellShowTemplate { get; }

	public BehaviorSubject<string> Field { get; }

	public bool? SortingAscending { get; set; }
}
