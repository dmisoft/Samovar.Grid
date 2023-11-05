using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public interface IPagingNavigationStrategy
        : INavigationStrategy
    {
        BehaviorSubject<int> PageSize { get; }
        BehaviorSubject<int> PagerSize { get; }
        BehaviorSubject<int> CurrentPage { get; }
        BehaviorSubject<int> TotalPageCount { get; }
        BehaviorSubject<DataGridPagerInfo> PagerInfo { get; }

        Task NavigateToNextPage();
        Task NavigateToPreviousPage();
        Task NavigateToPage(int pageNumber);

        Task NavigateToNextPager();
        Task NavigateToPreviousPager();
        Task NavigateToFirstPage();
        Task NavigateToLastPage();
    }
}
