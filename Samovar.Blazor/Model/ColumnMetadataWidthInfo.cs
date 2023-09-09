namespace Samovar.Blazor
{
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
}
