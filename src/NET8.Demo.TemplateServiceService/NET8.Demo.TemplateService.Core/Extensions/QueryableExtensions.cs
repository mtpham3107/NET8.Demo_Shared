using Microsoft.EntityFrameworkCore;
using NET8.Demo.Shared;
using System.Linq.Expressions;

namespace NET8.Demo.TemplateService.Core;

public static class QueryableExtensions
{
    public static async Task<PaginatedList<T>> ToPaginatedListAsync<T>(this IQueryable<T> query, int? pageIndex, int? pageSize)
    {
        var countResult = await query.CountAsync();

        return (await query.ToArrayAsync()).ToPaginatedList(pageIndex, pageSize, countResult);
    }

    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int? pageIndex, int? pageSize)
        => pageIndex.HasValue && pageSize.HasValue ? query.Skip(PaginationHelper.CalculateOffset(pageIndex.Value, pageSize.Value)).Take(pageSize.Value) : query;

    public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, bool condition, Expression<Func<T, bool>> predicate)
    {
        if (condition)
        {
            return source.Where(predicate);
        }
        return source;
    }
}
