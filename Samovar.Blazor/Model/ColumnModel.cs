using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;

namespace Samovar.Blazor;

public abstract class ColumnModel
: IColumnModel
{
	public string Id { get; } = $"columnmodel{Guid.NewGuid().ToString().Replace("-", "")}";
	public abstract ColumnType ColumnType { get; }
	public BehaviorSubject<double> Width { get; } = new BehaviorSubject<double>(50d);
	public int Order { get; set; }
	public string MainCellId { get; } = $"maincell{Guid.NewGuid().ToString().Replace("-", "")}";
	public string HeaderCellId { get; } = $"visiblecolcell{Guid.NewGuid().ToString().Replace("-", "")}";
	public string FilterCellId { get; } = $"filtercolcell{Guid.NewGuid().ToString().Replace("-", "")}";
	public string HiddenHeaderCellId { get; } = $"hiddencolcell{Guid.NewGuid().ToString().Replace("-", "")}";

	public IObservable<string> WidthStyle { get; }

	protected ColumnModel()
    {
		WidthStyle = Width.Select(w => $"width:{w.ToString(System.Globalization.CultureInfo.InvariantCulture)}px;");
	}
}

