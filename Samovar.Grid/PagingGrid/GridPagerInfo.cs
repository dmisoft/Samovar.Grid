namespace Samovar.Grid;

public readonly struct GridPagerInfo
{
    public static readonly GridPagerInfo Empty = new GridPagerInfo(0, 0, 0);

    public readonly uint StartPage;
    public readonly uint EndPage;
    public readonly uint CurrentPage;
    public readonly uint TotalPages;
    public readonly uint TotalItemsCount;

    public GridPagerInfo(uint startPage, uint endPage, uint currentPage, uint totalPages = 0, uint totalItemsCount = 0)
    {
        StartPage = startPage;
        EndPage = endPage;
        CurrentPage = currentPage;
        TotalPages = totalPages;
        TotalItemsCount = totalItemsCount;
    }
}
