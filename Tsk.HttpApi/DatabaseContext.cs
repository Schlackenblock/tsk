using Microsoft.EntityFrameworkCore;
using Tsk.HttpApi.Products;

namespace Tsk.HttpApi;

internal class DatabaseContext : DbContext
{
    public DbSet<ProductEntity> Products => Set<ProductEntity>();

    protected override void OnConfiguring(DbContextOptionsBuilder options) =>
        options.UseNpgsql("Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=postgres");
}
