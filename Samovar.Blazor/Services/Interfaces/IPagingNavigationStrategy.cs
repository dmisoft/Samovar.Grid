using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public interface IPagingNavigationStrategy
        : INavigationStrategy
    {
        ISubject<int> PageSize { get; }
        ISubject<int> PagerSize { get; }
        ISubject<int> CurrentPage { get; }
        ISubject<int> PageCount { get; }
        ISubject<DataGridPagerInfo> PagerInfo { get; }

        Task NavigateToNextPage();
        Task NavigateToPreviousPage();
        Task NavigateToPage(int pageNumber);

        Task NavigateToNextPager();
        Task NavigateToPreviousPager();
        Task NavigateToFirstPage();
        Task NavigateToLastPage();
    }
}
