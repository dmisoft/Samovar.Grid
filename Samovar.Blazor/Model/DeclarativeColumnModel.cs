using System.Reactive.Subjects;
using System.Text.RegularExpressions;

namespace Samovar.Blazor;

public abstract partial class DeclarativeColumnModel
	: ColumnModel, IDeclarativeColumnModel
{
	public BehaviorSubject<string> DeclaratedWidthParameter { get; set; } = new BehaviorSubject<string>("1*");
	public double DeclaratedWidth { get; set; }
	public DeclarativeColumnWidthMode DeclaratedWidthMode { get; set; }
	public BehaviorSubject<string> Title { get; } = new BehaviorSubject<string>("");

	protected DeclarativeColumnModel()
		: base()
	{
		DeclaratedWidthParameter.Subscribe(WidthSubscriber);
	}

	private void WidthSubscriber(string declaratedWidth)
	{
		bool isAbsoluteWidth = !string.IsNullOrEmpty(declaratedWidth) && IsAbsoluteWidth().IsMatch(declaratedWidth);
		bool isRelativeWidth = !string.IsNullOrEmpty(declaratedWidth) && IsRelativeWidth().IsMatch(declaratedWidth);

		if (isAbsoluteWidth)
		{
			DeclaratedWidth = double.Parse(declaratedWidth.Replace("px", ""));
			DeclaratedWidthMode = DeclarativeColumnWidthMode.Absolute;
		}
		else if (isRelativeWidth)
		{
			DeclaratedWidth = double.Parse(declaratedWidth.Replace("*", ""));
			DeclaratedWidthMode = DeclarativeColumnWidthMode.Relative;
		}
		else
		{
			DeclaratedWidth = 50;
			DeclaratedWidthMode = DeclarativeColumnWidthMode.Absolute;
		}
	}

	[GeneratedRegex(@"^[^0][0-9]*\*$")]
	private static partial Regex IsRelativeWidth();
	[GeneratedRegex("^[^0][0-9]*px$")]
	private static partial Regex IsAbsoluteWidth();
}

