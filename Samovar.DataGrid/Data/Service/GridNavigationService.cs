using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samovar.DataGrid.Data.Service
{
    internal class GridNavigationService
    {
        internal int CurrentPage { get; set; } = 0;
        internal int PagerSize { get; set; } = 5;

        internal int StartPage = 0;
        internal int EndPage = 0;
        internal int TotalPages = 0;
        internal int PageSize { set; get; } = 10;

        internal void CalculateInitPaginationInfo(int dataCount)
        {
            CurrentPage = 0;
            StartPage = 0;
            EndPage = 0;
            TotalPages = 0;

            if (dataCount > 0)
            {
                CurrentPage = 1;
                TotalPages = (int)Math.Ceiling(dataCount / (decimal)PageSize);
            }

            SetPagerSize("forward");
        }

        internal void SetPagerSize(string direction)
        {
            if (direction == "forward" && EndPage < TotalPages)
            {
                StartPage = EndPage + 1;
                if (EndPage + PagerSize < TotalPages)
                {
                    EndPage = StartPage + PagerSize - 1;
                }
                else
                {
                    EndPage = TotalPages;
                }
                CurrentPage = StartPage;
            }
            else if (direction == "back" && StartPage > 1)
            {
                EndPage = StartPage - 1;
                StartPage -= PagerSize;
                CurrentPage = EndPage;
            }
        }

        internal void NavigateToPage(string direction)
        {
            if (direction == "next")
            {
                if (CurrentPage < TotalPages)
                {
                    if (CurrentPage == EndPage)
                    {
                        SetPagerSize("forward");
                    }
                    else
                    {
                        CurrentPage += 1;
                    }
                }
            }
            else if (direction == "previous")
            {
                if (CurrentPage > 1)
                {
                    if (CurrentPage == StartPage)
                    {
                        SetPagerSize("back");
                    }
                    else
                    {
                        CurrentPage -= 1;
                    }
                }
            }
        }
    }
}
