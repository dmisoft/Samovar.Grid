namespace Samovar.DataGrid
{
    public class ColumnWidthChangeManager
    {
        public bool IsMouseDown { get; set; }
        public double StartMouseMoveX { get; set; } = double.NaN;
        public double EndMouseMoveX { get; set; } = double.NaN;
        public double OldAbsoluteVisibleWidthValue { get; set; } = double.NaN;
        public double OldAbsoluteEmptyColVisibleWidthValue { get; set; } = double.NaN;
        public ColumnMetadata MouseMoveCol { get; set; } = null;

        public void Reset()
        {
            IsMouseDown = false;
            StartMouseMoveX = double.NaN;
            EndMouseMoveX = double.NaN;
            OldAbsoluteVisibleWidthValue = double.NaN;
            OldAbsoluteEmptyColVisibleWidthValue = double.NaN;
        }
    }
}
