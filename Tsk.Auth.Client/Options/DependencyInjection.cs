using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Tsk.Auth.Client.Options;

internal static class DependencyInjection
{
    public static void AddTskAuthOptions(
        this IServiceCollection services,
        string configurationSectionPath,
        bool validateConfigurationOnStart)
    {
        var tskAuthOptionsBuilder = services
            .AddOptions<TskAuthOptions>()
            .BindConfiguration(configurationSectionPath)
            .ValidateDataAnnotations();

        if (validateConfigurationOnStart)
        {
            tskAuthOptionsBuilder.ValidateOnStart();
        }
    }

    public static void ReconfigureJwtBearerDefaults(this IServiceCollection services)
    {
        services.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, JwtBearerOptionsConfigurer>();
    }
}
