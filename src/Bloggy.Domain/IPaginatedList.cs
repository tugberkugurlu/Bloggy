namespace Bloggy.Domain
{
    public interface IPaginatedList<out T>
    {
        int PageIndex { get; }
        int PageSize { get; }
        int TotalCount { get; }
        int TotalPageCount { get; }

        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
    }
}