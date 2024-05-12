using Microsoft.EntityFrameworkCore;
using Tsk.Store.HttpApi.Products;

namespace Tsk.Store.HttpApi;

public class TskContext(DbContextOptions<TskContext> options) : DbContext(options)
{
    public DbSet<ProductEntity> Products => Set<ProductEntity>();
}
