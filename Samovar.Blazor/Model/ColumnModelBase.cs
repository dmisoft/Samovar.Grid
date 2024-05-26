using System.Reactive.Subjects;

namespace Samovar.Blazor
{
	public abstract partial class ColumnModelBase
        : IColumnModel
    {
        public string Id { get; } = $"columnmodel{Guid.NewGuid().ToString().Replace("-", "")}";
        public abstract ColumnType ColumnType { get; }
        public BehaviorSubject<string> Title { get; } = new BehaviorSubject<string>("");
        public Subject<string> Width { get; } = new Subject<string>();
        public int Order { get; set; }
        public string HeaderCellId { get; } = $"visiblecolcell{Guid.NewGuid().ToString().Replace("-", "")}";
        public string FilterCellId { get; } = $"filtercolcell{Guid.NewGuid().ToString().Replace("-", "")}";
        public string HiddenHeaderCellId { get; } = $"hiddencolcell{Guid.NewGuid().ToString().Replace("-", "")}";
        public double AbsoluteWidth { get; set; }
        public string WidthStyle
        {
            get
            {
                return $"width:{AbsoluteWidth.ToString(System.Globalization.CultureInfo.InvariantCulture)}px;";
            }
        }
    }
}
