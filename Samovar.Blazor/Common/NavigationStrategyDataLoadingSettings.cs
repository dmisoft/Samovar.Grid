using System;
using System.Collections.Generic;

namespace Samovar.Blazor
{
    public readonly struct NavigationStrategyDataLoadingSettings 
		: IEquatable<NavigationStrategyDataLoadingSettings>
	{
		public static readonly NavigationStrategyDataLoadingSettings Empty = new NavigationStrategyDataLoadingSettings(-1, -1);

		public static readonly NavigationStrategyDataLoadingSettings FetchAll = new NavigationStrategyDataLoadingSettings(0, -1, showAll: true);

		public readonly int Skip;

		public readonly int Take;

		public readonly bool ShowAll;

		public NavigationStrategyDataLoadingSettings(int skip, int take, bool showAll = false)
		{
			Skip = skip;
			Take = take;
			ShowAll = showAll;
		}

		public bool Equals(NavigationStrategyDataLoadingSettings other)
		{
			if (Skip == other.Skip)
			{
				return Take == other.Take;
			}
			return false;
		}

		public override string ToString()
		{
			return $"{{ Skip = {Skip}, Take = {Take} }}";
		}

		public bool IsEmpty()
		{
			return IsEmpty(this);
		}

		public static bool IsEmpty(NavigationStrategyDataLoadingSettings settings)
		{
			return EqualityComparer<NavigationStrategyDataLoadingSettings>.Default.Equals(settings, Empty);
		}

		//public static DataGridPagerSettings Create(IDataNavigationStrategy strategy)
		//{
		//	return FromOffsets(strategy.DataBindingSkipItems, strategy.DataBindingTakeItems);
		//}

		public static NavigationStrategyDataLoadingSettings FromOffsets(int skip, int take)
		{
			return new NavigationStrategyDataLoadingSettings(skip, take);
		}

		//public static DataGridPagerSettings FromPager(int index, int size, DataGridNavigationMode mode)
		//{
		//	if (mode != DataGridNavigationMode.ShowAllDataRows)
		//	{
		//		return FromOffsets(index * size, size);
		//	}
		//	return FetchAll;
		//}
	}
}
