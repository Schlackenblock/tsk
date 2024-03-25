using Microsoft.EntityFrameworkCore;
using Tsk.HttpApi.Products;

namespace Tsk.HttpApi;

public class TskContext(DbContextOptions<TskContext> options) : DbContext(options)
{
    public DbSet<ProductEntity> Products => Set<ProductEntity>();
}
