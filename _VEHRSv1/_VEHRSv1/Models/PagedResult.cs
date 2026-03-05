namespace _VEHRSv1.Models
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Results { get; set; } // List of results for the current page
        public int TotalRecords { get; set; } // Total number of records available
        public int PageSize { get; set; } // Number of records per page
        public int CurrentPage { get; set; } // Current page number
    }

}
