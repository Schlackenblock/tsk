using Microsoft.EntityFrameworkCore;
using Tsk.Store.HttpApi.Products;

namespace Tsk.Store.HttpApi;

public class TskContext : DbContext
{
    public TskContext(DbContextOptions<TskContext> options)
        : base(options)
    {
    }

    public DbSet<ProductEntity> Products => Set<ProductEntity>();
}
