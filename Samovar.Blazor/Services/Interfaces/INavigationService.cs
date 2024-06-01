using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public interface INavigationService
    {
        BehaviorSubject<NavigationMode> NavigationMode { get; }
        INavigationStrategy NavigationStrategy { get; }
    }
}
