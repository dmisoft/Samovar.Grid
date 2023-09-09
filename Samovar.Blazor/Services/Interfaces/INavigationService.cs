namespace Samovar.Blazor
{
    public interface INavigationService
    {
        ISubject<DataGridNavigationMode> NavigationMode { get; }
        INavigationStrategy NavigationStrategy { get; }
    }
}
