namespace Samovar.Blazor
{
    public readonly struct DataGridVirtualScrollingInfo
        : IEquatable<DataGridVirtualScrollingInfo>
    {
        public static readonly DataGridVirtualScrollingInfo Empty = new DataGridVirtualScrollingInfo(0d, 0d);
        public readonly double OffsetX;
        public readonly double OffsetY;

        public readonly double TopPlaceholderHeight;
        public readonly double BottomPlaceholderHeight;
        public readonly double ScrollContainerHeight;
        public readonly double ContentContainerHeight;

        public DataGridVirtualScrollingInfo(double offsetX, double offsetY, double contentContainerHeight = 0, double topPlaceholderHeight = 0, double bottomPlaceholderHeight = 0, double scrollContainerHeight = 0)
        {
            OffsetX = offsetX;
            OffsetY = offsetY;
            ContentContainerHeight = contentContainerHeight;
            TopPlaceholderHeight = topPlaceholderHeight;
            BottomPlaceholderHeight = bottomPlaceholderHeight;
            ScrollContainerHeight = scrollContainerHeight;
        }

        public bool Equals(DataGridVirtualScrollingInfo other)
        {
            return Math.Abs(OffsetX - other.OffsetX) < double.Epsilon && Math.Abs(OffsetY - other.OffsetY) < double.Epsilon && ContentContainerHeight.Equals(other.ContentContainerHeight);
        }
    }
}
