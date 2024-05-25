//namespace Samovar.Blazor
//{
//    public class ColumnMetadataWidthInfo
//    {
//        public DeclaratedColumnWidthMode DeclaratedWidthMode { get; set; } = DeclaratedColumnWidthMode.Relative;
//        public double WidthValue { get; set; } = 1;
//        public enum DeclaratedColumnWidthMode
//        {
//            Relative = 1,
//            Absolute = 2
//        }
//    }
//}

namespace Samovar.Blazor;
public enum DeclaratedColumnWidthMode
{
    Relative = 1,
    Absolute = 2
}
