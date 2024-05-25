namespace Samovar.Blazor
{
    public class DetailExpanderColumnModel
        : ColumnModelBase
    {
        public override DataGridColumnType ColumnType { get; } = DataGridColumnType.DetailExpanderColumn;

        public DetailExpanderColumnModel()
            : base()
        {
            double detailExpanderColWidth = 30;

            ColumnMetadataWidthInfo widthInfo = new ColumnMetadataWidthInfo();
            widthInfo.DeclaratedWidthMode = ColumnMetadataWidthInfo.DeclaratedColumnWidthMode.Absolute;
            widthInfo.WidthValue = detailExpanderColWidth;


            WidthInfo = widthInfo;
            VisibleAbsoluteWidthValue = detailExpanderColWidth;
        }
    }
}
