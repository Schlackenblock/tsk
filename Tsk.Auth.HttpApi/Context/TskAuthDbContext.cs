using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Tsk.Auth.HttpApi.Entities;

namespace Tsk.Auth.HttpApi.Context;

public sealed class TskAuthDbContext : DbContext
{
    public TskAuthDbContext(DbContextOptions<TskAuthDbContext> dbContextOptions)
        : base(dbContextOptions)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Session> Sessions => Set<Session>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var currentAssembly = Assembly.GetExecutingAssembly();
        modelBuilder.ApplyConfigurationsFromAssembly(currentAssembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
    }
}
