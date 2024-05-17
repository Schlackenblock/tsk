using Tsk.Auth.Client;
using Tsk.Auth.HttpApi.JwtAuth.Abstractions;

namespace Tsk.Auth.HttpApi.JwtAuth.JwtBearerImplementation;

public static class DependencyInjection
{
    public static void AddJwtTokenAuth(this WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Services.AddTskAuthentication();

        webApplicationBuilder.Services
            .AddOptions<JwtAuthOptions>()
            .BindConfiguration(nameof(JwtAuthOptions))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        webApplicationBuilder.Services
            .AddScoped<IJwtTokenIssuer, JwtTokenIssuer>()
            .AddScoped<IJwtRefreshTokenValidator, JwtRefreshTokenValidator>();

        webApplicationBuilder.Services.AddAuthorization();
    }
}
