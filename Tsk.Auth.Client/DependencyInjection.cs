using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Tsk.Auth.Client.Options;
using Tsk.Auth.Client.Sessions;

namespace Tsk.Auth.Client;

[PublicAPI]
public static class DependencyInjection
{
    public static AuthenticationBuilder AddTskAuthentication(
        this IServiceCollection services,
        Action<TskAuthConfiguration>? configure = null)
    {
        var configuration = new TskAuthConfiguration();
        configure?.Invoke(configuration);

        services.AddTskAuthOptions(
            configuration.ConfigurationSectionName,
            configuration.ValidateConfigurationOnStart
        );

        var authenticationBuilder = services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.ReconfigureJwtBearerDefaults();

        services.AddCurrentUserAccessor();

        return authenticationBuilder;
    }
}

[PublicAPI]
public sealed class TskAuthConfiguration
{
    public string ConfigurationSectionName { get; set; } = "TskAuthOptions";
    public bool ValidateConfigurationOnStart { get; set; } = true;
}
