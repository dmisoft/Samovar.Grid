using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public interface INavigationService
    {
        BehaviorSubject<DataGridNavigationMode> NavigationMode { get; }
        INavigationStrategy NavigationStrategy { get; }
        //BehaviorSubject<NavigationStrategyDataLoadingSettings> DataLoadingSettings { get; set; }
    }
}
