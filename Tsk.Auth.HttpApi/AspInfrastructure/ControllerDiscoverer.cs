using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Tsk.Auth.HttpApi.AspInfrastructure;

public sealed class ControllerDiscoverer : ControllerFeatureProvider
{
    protected override bool IsController(TypeInfo candidateType)
    {
        var apiControllerBaseType = typeof(ApiControllerBase);
        var isCustomController = !candidateType.IsAbstract && candidateType.IsAssignableTo(apiControllerBaseType);
        return isCustomController || base.IsController(candidateType);
    }
}

public static class ControllerDiscovererRegistration
{
    public static void ConfigureControllerDiscoverer(this IMvcBuilder mvcBuilder)
    {
        mvcBuilder.ConfigureApplicationPartManager(
            applicationPartManager =>
            {
                var controllerDiscoverer = new ControllerDiscoverer();
                applicationPartManager.FeatureProviders.Add(controllerDiscoverer);
            }
        );
    }
}
