using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace Tsk.Auth.HttpApi.JwtAuth.JwtBearerImplementation;

public static class DependencyInjection
{
    public static void AddJwtBearerAuth(this WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Services
            .AddSingleton<IPostConfigureOptions<JwtBearerOptions>, JwtBearerOptionsConfigurer>()
            .AddOptions<JwtAuthOptions>()
            .BindConfiguration(nameof(JwtAuthOptions))
            .ValidateDataAnnotations()
            .ValidateOnStart();


        webApplicationBuilder.Services
            .AddScoped<JwtTokenIssuer>()
            .AddScoped<JwtRefreshTokenValidator>()
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        webApplicationBuilder.Services.AddAuthorization();
    }
}
