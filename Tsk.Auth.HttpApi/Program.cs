using Microsoft.EntityFrameworkCore;
using Tsk.Auth.HttpApi.AspInfrastructure;
using Tsk.Auth.HttpApi.Context;
using Tsk.Auth.HttpApi.Swagger;

var webApplicationBuilder = WebApplication.CreateBuilder(args);

webApplicationBuilder
    .Services
    .AddDbContext<TskAuthContext>(
        options =>
        {
            var connectionString = webApplicationBuilder.Configuration.GetConnectionString("PostgreSQL");
            options.UseNpgsql(connectionString);
            options.UseSnakeCaseNamingConvention();
        }
    )
    .AddSwaggerGen(options => options.UseUniqueSchemaIds())
    .AddControllers()
    .ConfigureControllerDiscoverer();

var webApplication = webApplicationBuilder.Build();

webApplication.UseSwagger(Environments.Development);
webApplication.UseRouting();
webApplication.MapControllers();

webApplication.Run();
