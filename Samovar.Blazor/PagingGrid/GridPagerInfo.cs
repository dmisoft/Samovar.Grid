namespace Samovar.Blazor;

public readonly struct GridPagerInfo
{
    public static readonly GridPagerInfo Empty = new GridPagerInfo(0, 0, 0);

    public readonly int StartPage;
    public readonly int EndPage;
    public readonly int CurrentPage;
    public readonly int TotalPages;
    public readonly int TotalItemsCount;

    public GridPagerInfo(int startPage, int endPage, int currentPage, int totalPages = 0, int totalItemsCount = 0)
    {
        StartPage = startPage;
        EndPage = endPage;
        CurrentPage = currentPage;
        TotalPages = totalPages;
        TotalItemsCount = totalItemsCount;
    }
}
