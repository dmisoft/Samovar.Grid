using System.Reactive.Subjects;

namespace Samovar.Grid
{
    public interface IPagingNavigationStrategy
        : INavigationStrategy
    {
        BehaviorSubject<uint> PageSize { get; }
        BehaviorSubject<uint> PagerSize { get; }
        BehaviorSubject<uint> CurrentPage { get; }
        BehaviorSubject<uint> TotalPageCount { get; }
        BehaviorSubject<GridPagerInfo> PagerInfo { get; }

        Task NavigateToNextPage();
        Task NavigateToPreviousPage();
        Task NavigateToPage(uint pageNumber);

        Task NavigateToNextPager();
        Task NavigateToPreviousPager();
        Task NavigateToFirstPage();
        Task NavigateToLastPage();
    }
}
