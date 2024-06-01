namespace Samovar.Blazor;

public readonly struct NavigationStrategyDataLoadingSettings
    : IEquatable<NavigationStrategyDataLoadingSettings>
{
    public static readonly NavigationStrategyDataLoadingSettings Empty = new NavigationStrategyDataLoadingSettings(0, 0);

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
        return Skip == other.Skip && Take == other.Take;
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

    public static NavigationStrategyDataLoadingSettings FromOffsets(int skip, int take)
    {
        return new NavigationStrategyDataLoadingSettings(skip, take);
    }
}
