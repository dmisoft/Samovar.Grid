namespace Samovar.Blazor
{
    public interface INavigationStrategy
    {
        IObservable<Task<NavigationStrategyDataLoadingSettings>>? DataLoadingSettings { get; }
    }
}
