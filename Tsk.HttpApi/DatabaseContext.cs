using Microsoft.EntityFrameworkCore;
using Tsk.HttpApi.Products;

namespace Tsk.HttpApi;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<ProductEntity> Products => Set<ProductEntity>();
}
