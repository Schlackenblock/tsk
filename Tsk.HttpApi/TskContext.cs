using Microsoft.EntityFrameworkCore;
using Tsk.HttpApi.Products;

namespace Tsk.HttpApi;

public class TskContext : DbContext
{
    public TskContext(DbContextOptions<TskContext> options)
        : base(options)
    {
    }

    public DbSet<ProductEntity> Products => Set<ProductEntity>();
}
