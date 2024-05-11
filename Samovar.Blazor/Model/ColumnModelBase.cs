using System;
using System.Reactive.Subjects;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Samovar.Blazor
{
    public abstract class ColumnModelBase
        : IColumnModel
    {
        public string Id { get; } = $"columnmodel{Guid.NewGuid().ToString().Replace("-", "")}";


        public abstract DataGridColumnType ColumnType { get; }

        public BehaviorSubject<string> Title { get; } = new BehaviorSubject<string>("");

        public Subject<string> Width { get; } = new Subject<string>();

        public int ColumnOrder { get; set; }

        public string VisibleGridColumnCellId { get; } = $"visiblecolcell{Guid.NewGuid().ToString().Replace("-", "")}";
        public string FilterGridColumnCellId { get; } = $"filtercolcell{Guid.NewGuid().ToString().Replace("-", "")}";
        public string HiddenGridColumnCellId { get; } = $"hiddencolcell{Guid.NewGuid().ToString().Replace("-", "")}";
        public string EmptyDataColumnCellId { get; set; } = $"emptydatacell{Guid.NewGuid().ToString().Replace("-", "")}";

        public string FilterMenuContainerId { get; } = $"filtermenucontainer{Guid.NewGuid().ToString().Replace("-", "")}";
        public string FilterMenuId { get; set; } = $"filtermenu{Guid.NewGuid().ToString().Replace("-", "")}";
        
        public bool? SortingAscending { get; set; } = null;

        #region absolute column width

        public double VisibleAbsoluteWidthValue { get; set; }
        public string VisibleAbsoluteWidthCSS
        {
            get
            {
                return $"width:{VisibleAbsoluteWidthValue.ToString(System.Globalization.CultureInfo.InvariantCulture)}px;";
            }
        }

        public string VisibleWidthStyle
        {
            get
            {

                if (WidthInfo.WidthMode == ColumnMetadataWidthInfo.ColumnWidthMode.Absolute)
                {
                    return VisibleAbsoluteWidthCSS;
                }
                else
                {
                    return VisiblePercentWidthCSS;
                }
            }
        }
        #endregion

        #region relative column width
        public double VisiblePercentWidthValue { get; set; }
        public string VisiblePercentWidthCSS
        {
            get
            {
                return $"width:{VisiblePercentWidthValue.ToString(System.Globalization.CultureInfo.InvariantCulture)}%;";
            }
        }
        #endregion
        public ColumnMetadataWidthInfo WidthInfo { get; set; } = new ColumnMetadataWidthInfo();

        public PropertyInfo ColumnDataItemPropertyInfo => throw new NotImplementedException();

        public ColumnModelBase()
        {
            Width.Subscribe(WidthSubjectCallback);
        }

        private void WidthSubjectCallback(string widthValue)
        {
            bool isAbsoluteWidth = !string.IsNullOrEmpty(widthValue) && Regex.IsMatch(widthValue, "^[^0][0-9]*px$");
            bool isRelativeWidth = !string.IsNullOrEmpty(widthValue) && Regex.IsMatch(widthValue, @"^[^0][0-9]*\*$");
            if (isAbsoluteWidth)
            {
                WidthInfo.WidthMode = ColumnMetadataWidthInfo.ColumnWidthMode.Absolute;
                WidthInfo.WidthValue = int.Parse(widthValue.Replace("px", ""));
            }
            else if (isRelativeWidth)
            {
                WidthInfo.WidthMode = ColumnMetadataWidthInfo.ColumnWidthMode.Relative;
                WidthInfo.WidthValue = int.Parse(widthValue.Replace("*", ""));
            }
        }
    }
}
