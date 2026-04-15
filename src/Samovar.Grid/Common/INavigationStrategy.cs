using System.Reactive.Subjects;

namespace Samovar.Grid;

public interface INavigationStrategy
{
    BehaviorSubject<NavigationStrategyDataLoadingSettings> DataLoadingSettings { get; }
}
