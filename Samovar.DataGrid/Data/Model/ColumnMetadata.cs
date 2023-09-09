using System;

namespace Samovar.DataGrid
{
    public class ColumnMetadata
    {
        public ColumnMetadata()
        {
        }
        //data columns properties
        public Guid Id { get; set; }
        public int ColumnOrder { get; set; }
        public GridColumnType ColumnType { get; set; }
        public string Title { get; set; }
        public string Field { get; set; }

        //TODO gehört dem Command Typ
        public bool NewButtonVisible { get; set; } = true;
        //TODO gehört dem Command Typ
        public bool EditButtonVisible { get; set; } = true;
        //TODO gehört dem Command Typ
        public bool DeleteButtonVisible { get; set; } = true;

        #region absolute column width

        public double VisibleAbsoluteWidthValue { get; set; }
        private string VisibleAbsoluteWidthCSS {
            get { 
                return $"width:{VisibleAbsoluteWidthValue.ToString(System.Globalization.CultureInfo.InvariantCulture)}px;";
            } 
        }

        public string VisibleWidthStyle {
            get {

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
        public double VisiblePercentWidthValue { private get; set; }
        private string VisiblePercentWidthCSS {
            get {
                return $"width:{VisiblePercentWidthValue.ToString(System.Globalization.CultureInfo.InvariantCulture)}%;";
            }
        }
        #endregion

        public string VisibleGridColumnCellId { get; } = $"visiblecolcell{Guid.NewGuid().ToString().Replace("-", "")}";
        public string FilterGridColumnCellId { get; } = $"filtercolcell{Guid.NewGuid().ToString().Replace("-", "")}";
        public string HiddenGridColumnCellId { get; } = $"hiddencolcell{Guid.NewGuid().ToString().Replace("-", "")}";
        public string FilterMenuContainerId { get; } = $"filtermenucontainer{Guid.NewGuid().ToString().Replace("-","")}";
        public string FilterMenuId { get; set; } = $"filtermenu{Guid.NewGuid().ToString().Replace("-", "")}";

        public bool? SortingAscending { get; set; } = null;
        public ColumnMetadataWidthInfo WidthInfo { get; set; } = new ColumnMetadataWidthInfo();
    }

    public class ColumnMetadataWidthInfo
    {
        public ColumnWidthMode WidthMode { get; set; } = ColumnWidthMode.Relative;
        public double WidthValue { get; set; } = 1;
        public int MinWidthValue { get; set; } = 50;

        public enum ColumnWidthMode
        {
            Relative = 1,
            Absolute = 2
        }
    }

    public enum GridColumnType
    {
        Data,
        Command,
        DetailExpanderColumn,
        EmptyColumn
    }

    public class TempColumnMetadata
    {
        public TempColumnMetadata()
        {
        }
        public double VisibleAbsoluteWidthValue { get; set; }
        public double VisiblePercentWidthValue { get; set; }
    }
}
