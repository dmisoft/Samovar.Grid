using Microsoft.AspNetCore.Components;
using System.Reactive.Subjects;
using System.Reflection;

namespace Samovar.Blazor;

public interface IDataColumnModel
	: IDeclarativeColumnModel
{
	PropertyInfo ColumnDataItemPropertyInfo { get; }

	public BehaviorSubject<RenderFragment<object>?> CellShowTemplate { get; }

	public BehaviorSubject<RenderFragment<object>?> CellEditTemplate { get; }

	public BehaviorSubject<string> Field { get; }

	public BehaviorSubject<string> Title { get; }

	public bool? SortingAscending { get; set; }
}
