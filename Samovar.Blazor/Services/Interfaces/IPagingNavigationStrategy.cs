using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public interface IPagingNavigationStrategy
        : INavigationStrategy
    {
        BehaviorSubject<int> PageSize { get; }
        BehaviorSubject<int> PagerSize { get; }
        BehaviorSubject<int> CurrentPage { get; }
        BehaviorSubject<int> TotalPageCount { get; }
        BehaviorSubject<GridPagerInfo> PagerInfo { get; }

        Task NavigateToNextPage();
        Task NavigateToPreviousPage();
        Task NavigateToPage(int pageNumber);

        Task NavigateToNextPager();
        Task NavigateToPreviousPager();
        Task NavigateToFirstPage();
        Task NavigateToLastPage();
    }
}
