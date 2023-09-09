using System;

namespace Samovar.Blazor
{
    public readonly struct DataGridVirtualScrollingInfo
		: IEquatable<DataGridVirtualScrollingInfo>
	{
		public static readonly DataGridVirtualScrollingInfo Empty = new DataGridVirtualScrollingInfo(0d, 0d);


		public readonly double OffsetX;

		public readonly double OffsetY;

		public readonly string TranslatableDivHeight;
		

		public DataGridVirtualScrollingInfo(double offsetX, double offsetY, string translatableDivHeight = "")
		{
			OffsetX = offsetX;
			OffsetY = offsetY;
			TranslatableDivHeight = translatableDivHeight;
		}

		public bool Equals(DataGridVirtualScrollingInfo other)
		{
			return (OffsetX == other.OffsetX && OffsetY == other.OffsetY && TranslatableDivHeight.Equals(other.TranslatableDivHeight));
		}
	}
}
