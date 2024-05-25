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

            //ColumnMetadataWidthInfo widthInfo = new ColumnMetadataWidthInfo();
            DeclaratedWidthMode = DeclaratedColumnWidthMode.Absolute;
            DeclaratedWidth = detailExpanderColWidth;


            //WidthInfo = widthInfo;
            VisibleAbsoluteWidthValue = detailExpanderColWidth;
        }
    }
}
