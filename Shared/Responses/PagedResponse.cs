namespace LibraryManagement.Shared.Responses
{
    public record PagedResponse<T>(IEnumerable<T> Items, int TotalCount, int PageNumber, int PageSize) 
    {
        public int TotalPages =>
                (int)Math.Ceiling(TotalCount / (double)PageSize);

        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
