using Microsoft.EntityFrameworkCore;
using Tsk.HttpApi.Entities;
using Tsk.HttpApi.Products;

namespace Tsk.HttpApi;

public class TskDbContext : DbContext
{
    public TskDbContext(DbContextOptions<TskDbContext> options)
        : base(options)
    {
    }

    public DbSet<ProductEntity> Products => Set<ProductEntity>();
}
