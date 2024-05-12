using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Tsk.Auth.HttpApi.AspInfrastructure;
using Tsk.Auth.HttpApi.Context;
using Tsk.Auth.HttpApi.JwtAuth;
using Tsk.Auth.HttpApi.Swagger;

var webApplicationBuilder = WebApplication.CreateBuilder(args);

webApplicationBuilder.Services.AddDbContext<TskAuthContext>(
    options =>
    {
        var connectionString = webApplicationBuilder.Configuration.GetConnectionString("PostgreSQL");
        options.UseNpgsql(connectionString);
        options.UseSnakeCaseNamingConvention();
    }
);

webApplicationBuilder
    .Services
    .Configure<JwtAuthOptions>(webApplicationBuilder.Configuration.GetSection(nameof(JwtAuthOptions)))
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

webApplicationBuilder
    .Services
    .AddControllers()
    .ConfigureControllerDiscoverer();

webApplicationBuilder.Services.AddSwaggerGen(options => options.UseUniqueSchemaIds());

var webApplication = webApplicationBuilder.Build();

webApplication.UseSwagger(Environments.Development);
webApplication.UseRouting();

webApplication.UseAuthentication();
webApplication.UseAuthorization();

webApplication.MapControllers();

webApplication.Run();
