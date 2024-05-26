using Microsoft.AspNetCore.Components;
using System.Reactive.Subjects;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Samovar.Blazor;

public partial class DataColumnModel
	: ColumnModelBase, IDataColumnModel
{
	public override ColumnType ColumnType { get; } = ColumnType.Data;

	public BehaviorSubject<RenderFragment<object>?> CellShowTemplate { get; } = new BehaviorSubject<RenderFragment<object>?>(null);

	public BehaviorSubject<RenderFragment<object>?> CellEditTemplate { get; } = new BehaviorSubject<RenderFragment<object>?>(null);

	public BehaviorSubject<string> Field { get; } = new BehaviorSubject<string>("");

	public PropertyInfo ColumnDataItemPropertyInfo => throw new NotImplementedException();

	public bool? SortingAscending { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	public DeclarativeColumnWidthMode DeclaratedWidthMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	public double DeclaratedWidth { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

	protected DataColumnModel()
	{
		Width.Subscribe(WidthSubscriber);
	}

	private void WidthSubscriber(string widthValue)
	{
		bool isAbsoluteWidth = !string.IsNullOrEmpty(widthValue) && IsAbsoluteWidth().IsMatch(widthValue);
		bool isRelativeWidth = !string.IsNullOrEmpty(widthValue) && IsRelativeWidth().IsMatch(widthValue);

		if (isAbsoluteWidth)
		{
			DeclaratedWidthMode = DeclarativeColumnWidthMode.Absolute;
			DeclaratedWidth = double.Parse(widthValue.Replace("px", ""));
		}
		else if (isRelativeWidth)
		{
			DeclaratedWidthMode = DeclarativeColumnWidthMode.Relative;
			DeclaratedWidth = double.Parse(widthValue.Replace("*", ""));
		}
	}

	[GeneratedRegex(@"^[^0][0-9]*\*$")]
	private static partial Regex IsRelativeWidth();
	[GeneratedRegex("^[^0][0-9]*px$")]
	private static partial Regex IsAbsoluteWidth();
}
