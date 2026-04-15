namespace Samovar.Grid;

public readonly struct GridPagerInfo(uint startPage, uint endPage, uint currentPage, uint totalPages = 0, uint totalItemsCount = 0)
{
    public static readonly GridPagerInfo Empty = new GridPagerInfo(0, 0, 0);

    public readonly uint StartPage = startPage;
    public readonly uint EndPage = endPage;
    public readonly uint CurrentPage = currentPage;
    public readonly uint TotalPages = totalPages;
    public readonly uint TotalItemsCount = totalItemsCount;
}
