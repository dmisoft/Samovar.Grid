using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public interface INavigationStrategy
    {
        BehaviorSubject<NavigationStrategyDataLoadingSettings> DataLoadingSettings { get; }
    }
}
