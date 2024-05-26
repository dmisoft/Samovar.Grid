namespace Samovar.Blazor
{
    public class DetailExpanderColumnModel
        : ColumnModelBase
    {
        public override ColumnType ColumnType { get; } = ColumnType.DetailExpanderColumn;

        public DetailExpanderColumnModel()
            : base()
        {
            double detailExpanderColWidth = 30;

            //ColumnMetadataWidthInfo widthInfo = new ColumnMetadataWidthInfo();
            DeclaratedWidthMode = DeclarativeColumnWidthMode.Absolute;
            DeclaratedWidth = detailExpanderColWidth;


            //WidthInfo = widthInfo;
            AbsoluteWidth = detailExpanderColWidth;
        }
    }
}
