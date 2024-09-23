using static System.Math;

namespace NET8.Demo.Shared;

public static class PaginationHelper
{
    public static int CalculateOffset(int pageIndex, int pageSize)
    {
        pageIndex = Clamp(pageIndex, 0, pageIndex);

        return pageIndex is 0 ? 0 : (pageIndex - 1) * pageSize;
    }

    public static PaginatedList<T> ToPaginatedList<T>(this IEnumerable<T> data, int? pageIndex, int? pageSize, int totalCount)
        => new(pageIndex.HasValue && pageSize.HasValue && pageIndex > 0 ? data.Skip(CalculateOffset(pageIndex.Value, pageSize.Value)).Take(pageSize.Value) : data, pageIndex, pageSize, totalCount);
}
