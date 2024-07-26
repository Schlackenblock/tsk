using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Tsk.HttpApi.Entities;

namespace Tsk.HttpApi;

public class TskDbContext : DbContext
{
    public DbSet<ProductEntity> Products => Set<ProductEntity>();

    public TskDbContext(DbContextOptions<TskDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var currentAssembly = Assembly.GetExecutingAssembly();
        modelBuilder.ApplyConfigurationsFromAssembly(currentAssembly);
    }
}
