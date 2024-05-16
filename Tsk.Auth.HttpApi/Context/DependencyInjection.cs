using Microsoft.EntityFrameworkCore;

namespace Tsk.Auth.HttpApi.Context;

public static class TskAuthDbContextDependencyInjection
{
    public static void AddTskAuthDbContext(this WebApplicationBuilder webApplicationBuilder)
    {
        var postgreSqlConnectionString = webApplicationBuilder.Configuration.GetConnectionString("PostgreSQL");

        webApplicationBuilder.Services.AddDbContext<TskAuthDbContext>(options =>
        {
            options.UseNpgsql(postgreSqlConnectionString);
        });
    }
}
