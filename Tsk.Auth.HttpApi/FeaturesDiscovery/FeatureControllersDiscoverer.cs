using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Tsk.Auth.HttpApi.FeaturesDiscovery;

public sealed class FeatureControllersDiscoverer : ControllerFeatureProvider
{
    protected override bool IsController(TypeInfo candidateType)
    {
        var isCustomController = candidateType.IsAssignableTo(typeof(ApiControllerBase));
        if (isCustomController && !candidateType.IsAbstract)
        {
            return true;
        }

        return base.IsController(candidateType);
    }
}

public static class FeaturesDependencyInjection
{
    public static void AddFeatures(this WebApplicationBuilder webApplicationBuilder)
    {
        var featureControllersDiscoverer = new FeatureControllersDiscoverer();

        webApplicationBuilder.Services
            .AddControllers()
            .ReplaceDefaultFeatureProviders(featureControllersDiscoverer);
    }

    private static void ReplaceDefaultFeatureProviders(
        this IMvcBuilder mvcBuilder,
        IApplicationFeatureProvider customApplicationFeatureProvider)
    {
        mvcBuilder.ConfigureApplicationPartManager(applicationPartManager =>
        {
            applicationPartManager.FeatureProviders.Clear();
            applicationPartManager.FeatureProviders.Add(customApplicationFeatureProvider);
        });
    }
}
