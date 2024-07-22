namespace MintaProjekt.Models
{
    public class Pager
    {
        public int TotalItems { get; private set; } // Total number of records
        public int CurrentPage { get; private set; } // Active page
        public int PageSize { get; private set; } // Number of displayed records

        public int TotalPages { get; private set; } // Total number of pages
        public int StartPage { get; private set; } // First pageNumber displayed
        public int EndPage { get; private set; } // Last pageNumber displayed

        public Pager() { }

        public Pager(int totalItems, int page, int pageSize = 10)
        {
            int totalPages = (int)Math.Ceiling((double)totalItems / (double)pageSize);
            int currentPage = page;

            int startPage = currentPage - 5;
            int endPage = currentPage + 4;

            if(startPage <= 0) // If currentpage is close to startpage
            {
                endPage = endPage - ( startPage - 1 );
                startPage = 1;
            }

            if( endPage > totalPages) // Avoid displaying non-existing page
            {
                endPage = totalPages;
                if(endPage > 10)
                {
                    startPage = endPage - 9;
                }
            }

            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;

        }
    }
}
