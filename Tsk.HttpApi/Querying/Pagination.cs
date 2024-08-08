using Microsoft.EntityFrameworkCore;

namespace Tsk.HttpApi.Querying;

public class PaginationResult<T>
{
    public required List<T> Items { get; init; }
    public required int Count { get; init; }
}

public static class QueryablePaginationExtensions
{
    public static async Task<PaginationResult<T>> PaginateAsync<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize)
    {
        if (pageNumber < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageNumber), pageNumber, "Page number can't be less than 0.");
        }
        if (pageSize is < 1 or > 50)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), pageSize, "Page size must be between 1 and 50.");
        }

        var count = await query.CountAsync();

        var items = await query
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginationResult<T>
        {
            Items = items,
            Count = count
        };
    }
}
