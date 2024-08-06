namespace Tsk.HttpApi.Querying;

public static class QueryableOrderingExtensions
{
    public static IOrderedQueryable<TEntity> ApplyOrdering<TEntity>(
        this IQueryable<TEntity> query,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderer)
    {
        return orderer.Invoke(query);
    }
}
