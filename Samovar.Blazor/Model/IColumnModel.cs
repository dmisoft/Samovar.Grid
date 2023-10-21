using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public interface IColumnModel
    {
        public string Id { get; }
        public DataGridColumnType ColumnType { get; }
        public ISubject<string> Width { get; }
        public ColumnMetadataWidthInfo WidthInfo { get; set; }

        public int ColumnOrder { get; set; }
        public string VisibleWidthStyle { get; }
        public string VisibleAbsoluteWidthCSS { get; }
        public string VisiblePercentWidthCSS { get; }
        public double VisibleAbsoluteWidthValue { get; set; }
        public double VisiblePercentWidthValue { get; set; }

        public string VisibleGridColumnCellId { get; }
        public string FilterGridColumnCellId { get; }
        public string HiddenGridColumnCellId { get; }
        public string FilterMenuContainerId { get; }
        public string EmptyDataColumnCellId { get; }
        public string FilterMenuId { get; set; }


    }
}
