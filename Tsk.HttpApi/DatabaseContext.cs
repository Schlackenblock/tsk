using Microsoft.EntityFrameworkCore;
using Tsk.HttpApi.Products;

namespace Tsk.HttpApi;

public class DatabaseContext : DbContext
{
    public DbSet<ProductEntity> Products => Set<ProductEntity>();

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }
}
