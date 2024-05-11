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
            widthInfo.WidthMode = ColumnMetadataWidthInfo.ColumnWidthMode.Absolute;
            widthInfo.WidthValue = detailExpanderColWidth;


            WidthInfo = widthInfo;
            VisibleAbsoluteWidthValue = detailExpanderColWidth;
        }
    }
}
