namespace NET8.Demo.Shared;

public class PaginatedList<T>
{
    public PaginatedList(IEnumerable<T> items, int? pageIndex, int? pageSize, int totalCount)
    {
        Items = items;
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public IEnumerable<T> Items { get; set; }
    public int? PageIndex { get; private set; }
    public int? PageSize { get; private set; }
    public int TotalCount { get; private set; }
    public int TotalPageCount => PageSize.HasValue && PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : default;
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => (PageIndex + PageSize) < TotalPageCount;
}
