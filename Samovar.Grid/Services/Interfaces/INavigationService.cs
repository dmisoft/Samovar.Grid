using System.Reactive.Subjects;

namespace Samovar.Grid
{
    public interface INavigationService
    {
        BehaviorSubject<NavigationMode> NavigationMode { get; }
        INavigationStrategy NavigationStrategy { get; }
    }
}
