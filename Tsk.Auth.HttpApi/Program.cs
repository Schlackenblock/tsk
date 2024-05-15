using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tsk.Auth.HttpApi.AspInfrastructure;
using Tsk.Auth.HttpApi.Context;
using Tsk.Auth.HttpApi.JwtAuth;
using Tsk.Auth.HttpApi.Swagger;

var webApplicationBuilder = WebApplication.CreateBuilder(args);

webApplicationBuilder.Services.AddDbContext<TskAuthContext>(options =>
{
    var connectionString = webApplicationBuilder.Configuration.GetConnectionString("PostgreSQL");
    options.UseNpgsql(connectionString);
    options.UseSnakeCaseNamingConvention();
});

webApplicationBuilder.Services
    .AddOptions<JwtAuthOptions>()
    .BindConfiguration(nameof(JwtAuthOptions))
    .ValidateDataAnnotations()
    .ValidateOnStart();

webApplicationBuilder.Services
    .AddScoped<JwtTokenIssuer>()
    .AddScoped<JwtRefreshTokenValidator>();

webApplicationBuilder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

webApplicationBuilder.Services.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, JwtBearerOptionsConfigurer>();

webApplicationBuilder.Services.AddAuthorization();

webApplicationBuilder.Services.AddCurrentUserAccessor();

webApplicationBuilder.Services
    .AddControllers()
    .ConfigureControllerDiscoverer();

webApplicationBuilder.Services.AddSwaggerGen(options =>
{
    options.UseUniqueSchemaIds();
    options.AddJwtAuthentication();
});

var webApplication = webApplicationBuilder.Build();

webApplication.UseSwagger(Environments.Development);
webApplication.UseRouting();

webApplication.UseAuthentication();
webApplication.UseAuthorization();

webApplication.MapControllers();

webApplication.Run();
