namespace Samovar.Grid;

public readonly struct NavigationStrategyDataLoadingSettings(uint skip, uint take, bool showAll = false)
    : IEquatable<NavigationStrategyDataLoadingSettings>
{
    public static readonly NavigationStrategyDataLoadingSettings Empty = new NavigationStrategyDataLoadingSettings(0, 0);

    public static readonly NavigationStrategyDataLoadingSettings FetchAll = new NavigationStrategyDataLoadingSettings(0, 0, showAll: true);

    public readonly uint Skip = skip;

    public readonly uint Take = take;

    public readonly bool ShowAll = showAll;

    public bool Equals(NavigationStrategyDataLoadingSettings other)
    {
        return Skip == other.Skip && Take == other.Take && ShowAll == other.ShowAll;
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

    public static NavigationStrategyDataLoadingSettings FromOffsets(uint skip, uint take)
    {
        return new NavigationStrategyDataLoadingSettings(skip, take);
    }
}
